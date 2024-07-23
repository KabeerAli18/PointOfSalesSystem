using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PointOfSales.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApisPointOfSales.MiddleWares
{
    public class BearerTokenAuthHandler
    {
        private readonly RequestDelegate _next;

        public BearerTokenAuthHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip authentication for registration endpoint
            if (context.Request.Path.StartsWithSegments("/api/UsersManager/register"))
            {
                await _next(context);
                return;
            }

            // Check for Authorization header with Bearer token
            if (context.Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
            {
                var authHeader = authHeaderValues.ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();

                    // Validate and parse the token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    try
                    {
                        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = "ABC", // Replace with your issuer
                            ValidAudience = "XYZ", // Replace with your audience
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Kabeer000000#")) // Replace with your secret key
                        }, out SecurityToken validatedToken);

                        // Set the user principal
                        context.User = principal;

                        await _next(context);
                        return;
                    }
                    catch
                    {
                        // Token validation failed
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }
                }
            }

            // If no Bearer token, continue with Basic Authentication
            await _next(context);
        }
    }
}
