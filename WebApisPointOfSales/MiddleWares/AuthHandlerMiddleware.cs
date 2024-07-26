using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PointOfSales.Data;
using PointOfSales.Services;

namespace WebApisPointOfSales.MiddleWares
{
    public class AuthHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _realm;
        private readonly IServiceProvider _serviceProvider;
        private static readonly string Key = "b14ca5898a4e4133bbce2ea2315a1916"; // Should match the key used for encryption

        public AuthHandlerMiddleware(RequestDelegate next, string realm, IServiceProvider serviceProvider)
        {
            _next = next;
            _realm = realm;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip authentication for registration endpoint
            if (context.Request.Path.StartsWithSegments("/api/UsersManager/register"))
            {
                await _next(context);
                return;
            }

            // Check for AuthKey header
            if (!context.Request.Headers.TryGetValue("AuthKey", out var authKeyValues) || authKeyValues.ToString() != "DemoTrainingKey")
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            // Check for Authorization header
            if (!context.Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            var authHeader = authHeaderValues.ToString();
            if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            var encodedCreds = authHeader.Substring("Basic ".Length).Trim();
           // Console.WriteLine($"Encoded Credentials: {encodedCreds}");

            try
            {
                var creds = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCreds));
                //Console.WriteLine($"Decoded Credentials: {creds}");

                var uemailpwd = creds.Split(':');
                if (uemailpwd.Length != 2)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                var uemail = uemailpwd[0];
                var upassword = uemailpwd[1];

                Console.WriteLine($"Email: {uemail}");
                Console.WriteLine($"Password: {upassword}");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == uemail);

                    if (user == null || PasswordSecurityService.EncryptPassword(Key, upassword) != user.Password)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }
                }
            }
            catch (FormatException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            await _next(context);
        }
    }

}
