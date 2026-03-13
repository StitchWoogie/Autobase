using System;
using System.Security.Cryptography;
using System.Text;

namespace OpcUa.Client.Host
{
    public static class OpcUaCredentialProtection
    {
        public static string Protect(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = ProtectedData.Protect(
                bytes, null, DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(encrypted);
        }

        public static string Unprotect(string protectedBase64)
        {
            if (string.IsNullOrEmpty(protectedBase64))
                return string.Empty;

            byte[] encrypted = Convert.FromBase64String(protectedBase64);
            byte[] bytes = ProtectedData.Unprotect(
                encrypted, null, DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
