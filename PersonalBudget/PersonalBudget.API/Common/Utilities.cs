using System;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using PersonalBudget.API.DTO;
using System.Text;

namespace PersonalBudget.API.Common
{
    public class Utilities
    {
        private IConfiguration Configuration { get; }

        public Utilities(IConfiguration config)
        {
            Configuration = config;
        }
        private static string CreateSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }
        public string[] HashPassword(string password, string salt = null)
        {
            if (String.IsNullOrEmpty(salt))
                salt = CreateSalt(32);

            HashAlgorithm hashAlg = new SHA256CryptoServiceProvider();
            byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] bytHash = hashAlg.ComputeHash(bytValue);
            string hashedPassword = Convert.ToBase64String(bytHash);
            return new string[] { hashedPassword, salt };
        }

        public string GenerateToken(AccountDTO account)
        {
            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, Configuration.GetValue<string>("Jwt:Subject")),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Id", account.UserID),
                    new Claim("UserName", account.Username)
                   };
            var expiresIn = Configuration.GetValue<int>("Jwt:ExpiresInSeconds");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("Jwt:Key")));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(Configuration.GetValue<string>("Jwt:Issuer"),
                                             Configuration.GetValue<string>("Jwt:Audience"),
                                             claims, expires: DateTime.UtcNow.AddSeconds(expiresIn), signingCredentials: signIn);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
