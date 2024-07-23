using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PointOfSales;
using PointOfSales.Services;

namespace WebApisPointOfSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public TokenController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        public IActionResult GenerateToken([FromBody] UserCredentials userCredentials)
        {
            Console.WriteLine($"Attempting to generate token for user: {userCredentials.Email}");

            if (IsValidUser(userCredentials))
            {
                var token = GenerateJwtToken(userCredentials.Email);
                Console.WriteLine($"Token generated: {token}");
                return Ok(new { token });
            }

            Console.WriteLine($"Unauthorized attempt with credentials: {userCredentials.Email}");
            return Unauthorized();
        }

        private bool IsValidUser(UserCredentials userCredentials)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                var user = dbContext.Users.FirstOrDefault(u => u.Email == userCredentials.Email);

                if (user == null)
                {
                    Console.WriteLine("User not found in database.");
                    return false;
                }

                var encryptedPassword = PasswordSecurityHandler.EncryptPassword("b14ca5898a4e4133bbce2ea2315a1916", userCredentials.Password);
                Console.WriteLine($"Encrypted provided password: {encryptedPassword}");
                Console.WriteLine($"Stored password: {user.Password}");

                return encryptedPassword == user.Password;
            }
        }

        private string GenerateJwtToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Kabeer000000#"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "ABC",
                audience: "XYZ",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class UserCredentials
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
