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
    }
}
/*
"Key": "ThisIsAStrongSecretKey123!123123123123",
    "Issuer": "InventoryMgmtAPI"*/