using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LocalMain.OPCUA
{
    public sealed class OpcUaUserRecord
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; } // optional
        public string Description { get; set; }   
        public bool IsEnabled { get; set; }

        // Password hash fields (PBKDF2)
        public string SaltBase64 { get; set; }
        public string HashBase64 { get; set; }
        public int Iterations { get; set; }

        // Certificate auth
        public string CertificateThumbprint { get; set; }
        public string CertificateSubject { get; set; }

        // Permission flags
        public OpcUaUserPermissions Permissions { get; set; }

        //public string GroupId; // optional, nullable

        //public sealed class OpcUaUserGroup
        //{
        //    public string Id;           // "Operators"
        //    public string DisplayName;  // "Operators (Line A)"
        //    public OpcUaUserPermissions Permissions;
        //}

        /// <summary>
        /// Thumbprint 기준으로 사용자 검색
        /// </summary>
        public static OpcUaUserRecord FindByCertificateThumbprint(
            IEnumerable<OpcUaUserRecord> users,
            string thumbprint)
        {
            if (users == null)
                return null;

            if (string.IsNullOrWhiteSpace(thumbprint))
                return null;

            string norm = NormalizeThumbprint(thumbprint);

            return users.FirstOrDefault(u =>
                !string.IsNullOrWhiteSpace(u.CertificateThumbprint) &&
                string.Equals(
                    NormalizeThumbprint(u.CertificateThumbprint),
                    norm,
                    StringComparison.OrdinalIgnoreCase));
        }

        private static string NormalizeThumbprint(string t)
        {
            return (t ?? string.Empty)
                .Replace(" ", "")
                .Trim();
        }
    }

    [Flags]
    public enum OpcUaUserPermissions
    {
        None = 0,
        Connect = 1 << 0,
        Browse = 1 << 1,
        Read = 1 << 2,
        Write = 1 << 3,

        ReadOnly = Connect | Browse | Read,
        Full = Connect | Browse | Read | Write
    }

    public sealed class OpcUaUserStore
    {
        private readonly string _path;
        private List<OpcUaUserRecord> _cache;
        private readonly object _sync = new object();

        public OpcUaUserStore(string path)
        {
            _path = path;
        }

        public List<OpcUaUserRecord> Load()
        {
            if (!File.Exists(_path))
                return new List<OpcUaUserRecord>();

            string json = File.ReadAllText(_path, Encoding.UTF8);
            var list = JsonConvert.DeserializeObject<List<OpcUaUserRecord>>(json);
            return list ?? new List<OpcUaUserRecord>();
        }

        public void Save(List<OpcUaUserRecord> users)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path));

            // username unique normalize
            var normalized = users
                .Where(u => u != null && !string.IsNullOrWhiteSpace(u.UserName))
                .GroupBy(u => u.UserName.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.Last())
                .OrderBy(u => u.UserName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            string json = JsonConvert.SerializeObject(normalized, Formatting.Indented);
            File.WriteAllText(_path, json, Encoding.UTF8);
        }

        // -------- Password hashing helpers --------
        public static void SetPassword(OpcUaUserRecord user, string password)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (password == null) password = "";

            // PBKDF2 params
            const int saltSize = 16;    // 128-bit
            const int hashSize = 32;    // 256-bit
            const int iterations = 200_000; // reasonable baseline (adjust if needed)

            byte[] salt = new byte[saltSize];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
                hash = pbkdf2.GetBytes(hashSize);

            user.SaltBase64 = Convert.ToBase64String(salt);
            user.HashBase64 = Convert.ToBase64String(hash);
            user.Iterations = iterations;
        }

        public static bool VerifyPassword(OpcUaUserRecord user, string password)
        {
            if (user == null) return false;
            if (password == null) password = "";

            if (string.IsNullOrEmpty(user.SaltBase64) ||
                string.IsNullOrEmpty(user.HashBase64) ||
                user.Iterations <= 0)
                return false;

            byte[] salt = Convert.FromBase64String(user.SaltBase64);
            byte[] expected = Convert.FromBase64String(user.HashBase64);

            byte[] actual;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, user.Iterations))
                actual = pbkdf2.GetBytes(expected.Length);

            return FixedTimeEquals(expected, actual);
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= (a[i] ^ b[i]);
            return diff == 0;
        }

        public void Reload()
        {
            lock (_sync)
            {
                _cache = Load();
            }
        }

        public OpcUaUserRecord FindByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            lock (_sync)
            {
                if (_cache == null)
                    _cache = Load();

                return _cache.FirstOrDefault(u =>
                    string.Equals(
                        u.UserName,
                        userName,
                        StringComparison.OrdinalIgnoreCase));
            }
        }


    }
}
