using Microsoft.AspNetCore.Identity;

namespace Domain.Security
{
    public static class PasswordHasher
    {
        private static readonly PasswordHasher<object> Hasher = new();

        public static string HashPassword(string password)
        {
            return Hasher.HashPassword(null!, password);
        }

        public static bool VerifyPassword(string passwordHash, string password)
        {
            var result = Hasher.VerifyHashedPassword(null!, passwordHash, password);

            return result != PasswordVerificationResult.Failed;
        }
    }
}
