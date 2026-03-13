using Opc.Ua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LocalMain
{
    public static class OpcUaCertificateService
    {
        /// <summary>
        /// Rejected → Trusted
        /// </summary>
        public static async Task TrustAsync(
            ApplicationConfiguration config,
            string thumbprint)
        {
            await MoveAsync(
                config,
                fromRejected: true,
                thumbprint);
        }

        /// <summary>
        /// Trusted → Rejected
        /// </summary>
        public static async Task RejectAsync(
            ApplicationConfiguration config,
            string thumbprint)
        {
            await MoveAsync(
                config,
                fromRejected: false,
                thumbprint);
        }

        /// <summary>
        /// Internal move logic (Store ↔ Store)
        /// </summary>
        private static async Task MoveAsync(
            ApplicationConfiguration config,
            bool fromRejected,
            string thumbprint)
        {
            CertificateStoreIdentifier srcStoreId =
                      fromRejected
                      ? config.SecurityConfiguration.RejectedCertificateStore
                      : config.SecurityConfiguration.TrustedPeerCertificates;

            CertificateStoreIdentifier dstStoreId =
                fromRejected
                ? config.SecurityConfiguration.TrustedPeerCertificates
                : config.SecurityConfiguration.RejectedCertificateStore;

            X509Certificate2 cert;

            // 1. Source store에서 단일 인증서 찾기
            using (var srcStore = srcStoreId.OpenStore(null))
            {
                cert = await srcStore
                    .GetSingleByThumbprintAsync(thumbprint)
                    .ConfigureAwait(false);

                if (cert == null)
                    throw new InvalidOperationException("Certificate not found.");

                await srcStore
                    .DeleteAsync(thumbprint)
                    .ConfigureAwait(false);
            }

            // 2. Destination store에 추가
            using (var dstStore = dstStoreId.OpenStore(null))
            {
                await dstStore
                    .AddAsync(cert)
                    .ConfigureAwait(false);
            }
        }
    }

    public static class CertificateStoreExtensions
    {
        /// <summary>
        /// Store 내부에서 Thumbprint로 단일 인증서 검색
        /// (컬렉션은 외부로 노출되지 않음)
        /// </summary>
        public static async Task<X509Certificate2>
           GetSingleByThumbprintAsync(
               this ICertificateStore store,
               string thumbprint)
        {
            var certs = await store.EnumerateAsync();

            foreach (X509Certificate2 cert in certs)
            {
                if (string.Equals(
                    cert.Thumbprint,
                    thumbprint,
                    StringComparison.OrdinalIgnoreCase))
                    return cert;
            }

            return null;
        }
    }

}
