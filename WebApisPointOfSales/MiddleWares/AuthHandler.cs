using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PointOfSales.Services;
using PointOfSales;

namespace WebApisPointOfSales.MiddleWares
{
    //public class AuthHandler
    //{
    //    private readonly RequestDelegate _next;
    //    private readonly string _realm;
    //    private readonly IServiceProvider _serviceProvider;
    //    private static readonly string Key = "b14ca5898a4e4133bbce2ea2315a1916"; // Should match the key used for encryption

    //    public AuthHandler(RequestDelegate next, string realm, IServiceProvider serviceProvider)
    //    {
    //        _next = next;
    //        _realm = realm;
    //        _serviceProvider = serviceProvider;
    //    }

    //    //public async Task InvokeAsync(HttpContext context)
    //    //{
    //    //    // Skip authentication for registration endpoint
    //    //    if (context.Request.Path.StartsWithSegments("/api/UsersManager/register"))
    //    //    {
    //    //        await _next(context);
    //    //        return;
    //    //    }

    //    //    string authTrainingKey;

    //    //    if (!context.Request.Headers.TryGetValue("DemoTrainingKey", out var demoTrainingKeyValues))
    //    //    {
    //    //        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
    //    //        {
    //    //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    //    //            await context.Response.WriteAsync("Unauthorized");
    //    //            return;
    //    //        }

    //    //        var authHeader = authHeaderValues.ToString();
    //    //        if (!authHeader.StartsWith("Basic "))
    //    //        {
    //    //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    //    //            await context.Response.WriteAsync("Unauthorized");
    //    //            return;
    //    //        }

    //    //        var encodedCreds = authHeader.Substring("Basic ".Length).Trim();
    //    //        var creds = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCreds));
    //    //        authTrainingKey = $"Basic {encodedCreds}";

    //    //        // KeySetting, but we have Authorization header, use it.
    //    //        context.Request.Headers["AuthKey"] = authTrainingKey;
    //    //    }
    //    //    else
    //    //    {
    //    //        authTrainingKey = demoTrainingKeyValues.ToString();
    //    //    }

    //    //    var encodedDemoTrainingKey = authTrainingKey.Substring("Basic ".Length).Trim();
    //    //    var demoCreds = Encoding.UTF8.GetString(Convert.FromBase64String(encodedDemoTrainingKey));
    //    //    string[] uemailpwd = demoCreds.Split(':');

    //    //    var uemail = uemailpwd[0];
    //    //    var upassword = uemailpwd[1];

    //    //    using (var scope = _serviceProvider.CreateScope())
    //    //    {
    //    //        var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    //    //        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == uemail);

    //    //        if (user == null || PasswordSecurityHandler.EncryptPassword(Key, upassword) != user.Password)
    //    //        {
    //    //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    //    //            await context.Response.WriteAsync("Unauthorized");
    //    //            return;
    //    //        }
    //    //    }

    //    //    await _next(context);
    //    //}

    //    public async Task InvokeAsync(HttpContext context)
    //    {
    //        // Log raw headers for debugging
    //        Console.WriteLine($"Authorization Header: {context.Request.Headers["Authorization"]}");

    //        // Skip authentication for registration endpoint
    //        if (context.Request.Path.StartsWithSegments("/api/UsersManager/register"))
    //        {
    //            await _next(context);
    //            return;
    //        }

    //        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
    //        {
    //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    //            await context.Response.WriteAsync("Unauthorized");
    //            return;
    //        }

    //        var authHeader = authHeaderValues.ToString();
    //        Console.WriteLine($"Auth Header Value: {authHeader}");

    //        if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
    //        {
    //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    //            await context.Response.WriteAsync("Unauthorized");
    //            return;
    //        }

    //        var encodedCreds = authHeader.Substring("Basic ".Length).Trim();
    //        Console.WriteLine($"Encoded Credentials: {encodedCreds}");

    //        try
    //        {
    //            var creds = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCreds));
    //            Console.WriteLine($"Decoded Credentials: {creds}");

    //            var uemailpwd = creds.Split(':');

    //            if (uemailpwd.Length != 2)
    //            {
    //                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    //                await context.Response.WriteAsync("Unauthorized");
    //                return;
    //            }

    //            var uemail = uemailpwd[0];
    //            var upassword = uemailpwd[1];

    //            Console.WriteLine($"Email: {uemail}");
    //            Console.WriteLine($"Password: {upassword}");

    //            // Your existing user validation logic here...
    //        }
    //        catch (FormatException)
    //        {
    //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    //            await context.Response.WriteAsync("Unauthorized");
    //        }

    //        await _next(context);
    //    }

    //}


    public class AuthHandler
    {
        private readonly RequestDelegate _next;
        private readonly string _realm;
        private readonly IServiceProvider _serviceProvider;
        private static readonly string Key = "b14ca5898a4e4133bbce2ea2315a1916"; // Should match the key used for encryption

        public AuthHandler(RequestDelegate next, string realm, IServiceProvider serviceProvider)
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

                    if (user == null || PasswordSecurityHandler.EncryptPassword(Key, upassword) != user.Password)
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
