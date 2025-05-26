using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProyectoFinal.Helpers;
using ProyectoFinal.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProyectoFinal.Service
{
    public class JwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(int userId, string correo)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var now = DateTime.Now;
            // Hora UTC
            var nowUtc = DateTime.Now;
            Console.WriteLine($"[JwtService] DateTime.UtcNow = {nowUtc:O}");

            // Construyendo descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, correo)
        }),
                NotBefore = now, // ← Válido desde ahora
                Expires = now.AddMinutes(_jwtSettings.ExpirationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key),
        SecurityAlgorithms.HmacSha256Signature)
            };

            Console.WriteLine($"[JwtService] TokenDescriptor.NotBefore = {tokenDescriptor.NotBefore:O}");
            Console.WriteLine($"[JwtService] TokenDescriptor.Expires   = {tokenDescriptor.Expires:O}");

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}