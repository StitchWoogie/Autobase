using Opc.Ua;
using OpcUa.Client.Abstractions;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace OpcUa.Client.Host
{
    public static class OpcUaIdentityBuilder
    {
        public static IUserIdentity Build(
            OpcUaServerDefinition def,
            ApplicationConfiguration config)
        {
            if (def == null)
                return new UserIdentity(new AnonymousIdentityToken());

            switch (def.AuthMode)
            {
                case OpcUaAuthMode.UserName:
                    string password = OpcUaCredentialProtection.Unprotect(
                        def.AuthPasswordProtected);
                    var userNameToken = new UserNameIdentityToken
                    {
                        UserName = def.AuthUserName ?? string.Empty,
                        DecryptedPassword = Encoding.UTF8.GetBytes(password ?? string.Empty)
                    };
                    return new UserIdentity(userNameToken);

                case OpcUaAuthMode.Certificate:
                    var cert = FindCertificate(def, config);
                    if (cert == null)
                        throw new InvalidOperationException(
                            $"User auth certificate not found. Thumbprint: {def.AuthCertificateThumbprint}, File: {def.AuthCertificateFilePath}");
                    if (!cert.HasPrivateKey)
                        throw new InvalidOperationException(
                            "Certificate does not contain a private key. A PFX/P12 file with private key is required for certificate authentication.");
                    return new UserIdentity(cert);

                default:
                    return new UserIdentity(new AnonymousIdentityToken());
            }
        }

        private static X509Certificate2 FindCertificate(
            OpcUaServerDefinition def,
            ApplicationConfiguration config)
        {
            // 1. Try loading from file path (PFX/P12 with private key)
            if (!string.IsNullOrEmpty(def.AuthCertificateFilePath) &&
                File.Exists(def.AuthCertificateFilePath))
            {
                try
                {
                    string password = OpcUaCredentialProtection.Unprotect(
                        def.AuthCertPasswordProtected);

                    var cert = new X509Certificate2(
                        def.AuthCertificateFilePath,
                        string.IsNullOrEmpty(password) ? (string)null : password,
                        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);

                    if (cert.HasPrivateKey)
                        return cert;

                    cert.Dispose();
                }
                catch
                {
                    // Fall through to thumbprint lookup
                }
            }

            // 2. Lookup by thumbprint in the client's own PKI store
            if (!string.IsNullOrEmpty(def.AuthCertificateThumbprint) && config != null)
            {
                var storeId = config.SecurityConfiguration.ApplicationCertificate;

                using (var store = storeId.OpenStore(null))
                {
                    var cert = store.GetSingleByThumbprintAsync(
                        def.AuthCertificateThumbprint)
                        .GetAwaiter().GetResult();

                    if (cert != null && cert.HasPrivateKey)
                        return cert;
                }

                // Also try the trusted peer store
                var trustedId = config.SecurityConfiguration.TrustedPeerCertificates;
                using (var store = trustedId.OpenStore(null))
                {
                    var cert = store.GetSingleByThumbprintAsync(
                        def.AuthCertificateThumbprint)
                        .GetAwaiter().GetResult();

                    if (cert != null)
                        return cert;
                }
            }

            return null;
        }
    }
}
