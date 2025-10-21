using System;
using System.Security.Cryptography;
using System.Text;
using SMS.Core.Interfaces;

namespace SMS.Infrastructure.Services
{
    // PBKDF2 with embedded parameters, format:
    // PBKDF2$<iterations>$<saltBase64>$<hashBase64>
    public class PasswordHasherService : IPasswordHasher
    {
        private const int SaltSize = 16;     // 128-bit
        private const int KeySize = 32;      // 256-bit
        private const int Iterations = 100000;

        public string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(KeySize);
            return $"PBKDF2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
        }

        public bool VerifyPassword(string passwordHash, string password)
        {
            if (string.IsNullOrWhiteSpace(passwordHash)) return false;

            var parts = passwordHash.Split('$');
            if (parts.Length != 4 || parts[0] != "PBKDF2") return false;

            if (!int.TryParse(parts[1], out var iterations)) return false;
            var salt = Convert.FromBase64String(parts[2]);
            var key = Convert.FromBase64String(parts[3]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var keyToCheck = pbkdf2.GetBytes(key.Length);

            return CryptographicOperations.FixedTimeEquals(keyToCheck, key);
        }
    }
}