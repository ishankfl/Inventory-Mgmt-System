using Inventory_Mgmt_System.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inventory_Mgmt_System.Utils
{
    public class JwtUtils
    {
        public static string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsAStrongSecretKey123!123123123123");

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iss,"InventoryMgmtAPI"),
        new Claim(JwtRegisteredClaimNames.Aud, "InventoryMgmtAPI"),
        new Claim("id", user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.Role == 0 ? "Admin" : "Staff"),
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = "InventoryMgmtAPI",
                Audience = "InventoryMgmtAPI",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public static Guid GetUserIdFromToken(string token)
        {
            var principal = GetPrincipalFromToken(token);
            var idClaim = principal?.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(idClaim))
                throw new UnauthorizedAccessException("User ID claim not found in token");

            return Guid.Parse(idClaim);
        }

        public static string GetRoleFromToken(string token)
        {
            var principal = GetPrincipalFromToken(token);
            var roleClaim = principal?.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(roleClaim))
                throw new UnauthorizedAccessException("Role claim not found in token");

            return roleClaim;
        }
        private static ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsAStrongSecretKey123!123123123123");

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = "InventoryMgmtAPI",
                ValidAudience = "InventoryMgmtAPI",
                ClockSkew = TimeSpan.Zero // optional: no clock skew
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null; // invalid token, could also throw if you prefer
            }
        }
    }
}
/*
"Key": "ThisIsAStrongSecretKey123!123123123123",
    "Issuer": "InventoryMgmtAPI"*/