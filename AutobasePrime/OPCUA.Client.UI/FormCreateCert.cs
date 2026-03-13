using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OPCUA.Client.UI
{
    public partial class FormCreateCert : Form
    {
        private readonly string _pkiRoot =
    Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OPCUA", "Client", "pki");

        private ToolTip _toolTip;

        public FormCreateCert()
        {
            InitializeComponent();

            textBoxPassword.Enabled = checkBoxEncrypt.Checked;
        }

        private void buttonGenerateKey_Click(object sender, EventArgs e)
        {
            try
            {
                string commonName = textBoxCN.Text.Trim();
                string organization = textBoxOrg.Text.Trim();
                string uri = textBoxURI.Text.Trim();
                string dns = textBoxDNS.Text.Trim();

                if (string.IsNullOrEmpty(commonName))
                    throw new InvalidOperationException("Common Name이 필요합니다.");

                if (string.IsNullOrEmpty(uri))
                    throw new InvalidOperationException("Application URI가 필요합니다.");

                if (string.IsNullOrEmpty(dns))
                    dns = System.Net.Dns.GetHostName();

                int years = (int)numYears.Value;
                bool usePassword = checkBoxEncrypt.Checked;
                string password = textBoxPassword.Text;

                string outputDir = Path.Combine(_pkiRoot, "generated",
                    $"{commonName}_{DateTime.Now:yyyyMMdd_HHmmss}");

                Directory.CreateDirectory(outputDir);

                // BouncyCastle로 인증서 생성
                var (bcCert, bcKeyPair) = GenerateCertificateWithBouncyCastle(
                    commonName, organization, uri, dns, years);


                SaveMinimalFormats(
              bcCert,
              bcKeyPair,
              outputDir,
              commonName,
              usePassword,
              password);

                MessageBox.Show(
                    $"인증서 생성 완료!\n\n저장 위치: {outputDir}",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                System.Diagnostics.Process.Start("explorer.exe", outputDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private (Org.BouncyCastle.X509.X509Certificate, AsymmetricCipherKeyPair)
            GenerateCertificateWithBouncyCastle(
            string commonName,
            string organization,
            string uri,
            string dns,
            int years)
        {
            var random = new SecureRandom();

            // ✅ RSA 3072 (산업 권장)
            var keyGen = new RsaKeyPairGenerator();
            keyGen.Init(new KeyGenerationParameters(random, 3072));
            var keyPair = keyGen.GenerateKeyPair();

            var certGen = new X509V3CertificateGenerator();

            // ✅ Secure Serial
            certGen.SetSerialNumber(new BigInteger(160, random).Abs());

            var subjectDN = new X509Name($"CN={commonName}, O={organization}");
            certGen.SetIssuerDN(subjectDN);
            certGen.SetSubjectDN(subjectDN);

            certGen.SetNotBefore(DateTime.UtcNow.AddMinutes(-5));
            certGen.SetNotAfter(DateTime.UtcNow.AddYears(years));

            certGen.SetPublicKey(keyPair.Public);

            // ✅ Basic Constraints
            certGen.AddExtension(
                X509Extensions.BasicConstraints,
                true,
                new BasicConstraints(false));

            // ✅ Key Usage
            certGen.AddExtension(
                X509Extensions.KeyUsage,
                true,
                 new KeyUsage(
                    KeyUsage.DigitalSignature |
                    KeyUsage.KeyEncipherment |
                    KeyUsage.NonRepudiation |
                    KeyUsage.DataEncipherment));

            // ✅ EKU (중요)
            certGen.AddExtension(
                X509Extensions.ExtendedKeyUsage,
                false,
                new ExtendedKeyUsage(new[]
                {
                    KeyPurposeID.id_kp_clientAuth,
                    KeyPurposeID.id_kp_serverAuth
                }));

            // ✅ SAN (URI + DNS)
            var san = new Asn1EncodableVector();
            san.Add(new GeneralName(GeneralName.UniformResourceIdentifier, uri));
            san.Add(new GeneralName(GeneralName.DnsName, dns));

            certGen.AddExtension(
                X509Extensions.SubjectAlternativeName,
                false,
                new DerSequence(san));

            // ✅ SKI / AKI
            var extUtils = new X509ExtensionUtilities();

            // Subject Key Identifier
            certGen.AddExtension(
                X509Extensions.SubjectKeyIdentifier,
                false,
                X509ExtensionUtilities.CreateSubjectKeyIdentifier(keyPair.Public));

            certGen.AddExtension(
                X509Extensions.AuthorityKeyIdentifier,
                false,
                X509ExtensionUtilities.CreateAuthorityKeyIdentifier(keyPair.Public));

            // ✅ SHA256
            var signatureFactory = new Asn1SignatureFactory(
                "SHA256WITHRSA",
                keyPair.Private,
                random);

            var cert = certGen.Generate(signatureFactory);

            return (cert, keyPair);
        }

        private void SaveMinimalFormats(
            Org.BouncyCastle.X509.X509Certificate bcCert,
            AsymmetricCipherKeyPair keyPair,
            string outputDir,
            string commonName, bool usePassword,
            string password)
        {
            // DER
            File.WriteAllBytes(
                Path.Combine(outputDir, $"{commonName}.der"),
                bcCert.GetEncoded());

            // PFX
            var store = new Pkcs12StoreBuilder().Build();
            var certEntry = new X509CertificateEntry(bcCert);

            store.SetCertificateEntry(commonName, certEntry);
            store.SetKeyEntry(
                commonName,
                new AsymmetricKeyEntry(keyPair.Private),
                new[] { certEntry });

            using (var fs = new FileStream(
              Path.Combine(outputDir, $"{commonName}.pfx"),
              FileMode.Create))
            {
                char[] pwd;

                if (usePassword)
                {
                    if (string.IsNullOrWhiteSpace(password))
                        throw new InvalidOperationException("Input the password.");

                    pwd = password.ToCharArray();
                }
                else
                {
                    pwd = new char[0]; // 암호 없음
                }

                store.Save(fs, pwd, new SecureRandom());
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonGoDir_Click(object sender, EventArgs e)
        {
            string generatedDir = Path.Combine(_pkiRoot, "generated");

            if (Directory.Exists(generatedDir))
                System.Diagnostics.Process.Start("explorer.exe", generatedDir);
            else
                System.Diagnostics.Process.Start("explorer.exe", _pkiRoot);
        }


        private void checkBoxEncrypt_CheckedChanged(object sender, EventArgs e)
        {
            bool on = checkBoxEncrypt.Checked;
            textBoxPassword.Enabled = on;
        }

    }

}
