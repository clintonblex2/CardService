using CardService.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace CardService.Application.Common.Helpers
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly string _secretKey;

        public PasswordHasher(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:SecretKey"];
        }

        public string HashPassword(string password)
        {
            using (var hmac = new HMACSHA256(System.Text.Encoding.UTF8.GetBytes(_secretKey)))
            {
                var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = hmac.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
