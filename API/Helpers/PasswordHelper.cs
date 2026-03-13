using System.Security.Cryptography;

namespace API.Helpers
{
    internal static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
            byte[] combined = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, combined, salt.Length, hash.Length);
            return Convert.ToBase64String(combined);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            byte[] combined = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[16];
            byte[] storedHashBytes = new byte[combined.Length - 16];
            Buffer.BlockCopy(combined, 0, salt, 0, 16);
            Buffer.BlockCopy(combined, 16, storedHashBytes, 0, storedHashBytes.Length);
            byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
            return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
        }
    }
}