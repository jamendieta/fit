using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Core.Entities;

namespace TaskManagement.Presentation.Security;

public class JwtTokenService
{
    private const string Secret = "task-management-super-secret-key-2026-change-me";
    private readonly byte[] _key = Encoding.UTF8.GetBytes(Secret);

    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new("userId", user.Id.ToString()),
            new(ClaimTypes.Name, user.Username)
        };

        var credentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "TaskManagementApi",
            audience: "TaskManagementClient",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = "TaskManagementApi",
            ValidAudience = "TaskManagementClient",
            IssuerSigningKey = new SymmetricSecurityKey(_key),
            ClockSkew = TimeSpan.Zero
        };
    }
}
