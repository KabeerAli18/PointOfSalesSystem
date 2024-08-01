using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using POS.API.MODEL.Users;

/// <summary>
/// This is the Custom Authentication Middle ware using Basic Auth with Key services.
/// </summary>
namespace POS.API.MiddleWares
{
    public class AuthBearerMiddleware
    {
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public AuthBearerMiddleware(string jwtKey, string jwtIssuer, string jwtAudience)
        {
            _jwtKey = jwtKey ?? throw new ArgumentNullException(nameof(jwtKey));
            _jwtIssuer = jwtIssuer ?? throw new ArgumentNullException(nameof(jwtIssuer));
            _jwtAudience = jwtAudience ?? throw new ArgumentNullException(nameof(jwtAudience));
        }

        public string GenerateJwtToken(Users user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.GetName()),
                    new Claim(ClaimTypes.Role, user.UserRole),
                    new Claim("id", user.GetEmail())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
