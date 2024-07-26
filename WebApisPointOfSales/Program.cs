using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PointOfSales.Services;
using WebApisPointOfSales.MiddleWares;
using Microsoft.Extensions.Logging;
using PointOfSales.Interfaces;
using Microsoft.AspNetCore.Authentication.OAuth;
using PointOfSales.Data;

namespace PointOfSales
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure log4net
            builder.Logging.ClearProviders();
            var log4netConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "log4net.config");
            builder.Logging.AddLog4Net(log4netConfigPath);

            // Add services to the container
            builder.Services.AddControllers();

            // Configure Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register MyDbContext with the DI container
            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseInMemoryDatabase("PointOfSalesDatabase"));

            // Configure JWT authentication
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = jwtSettings.GetValue<string>("Key") ?? throw new ArgumentNullException(nameof(jwtSettings));
            var issuer = jwtSettings.GetValue<string>("Issuer") ?? throw new ArgumentNullException(nameof(jwtSettings));
            var audience = jwtSettings.GetValue<string>("Audience") ?? throw new ArgumentNullException(nameof(jwtSettings));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });

            // Register AuthService
           builder.Services.AddSingleton(new AuthBearerMiddleware(key, issuer, audience));
           builder.Services.AddAutoMapper(typeof(MappingProfile)); // Add AutoMapper


            // Register services for User Interfaces
            builder.Services.AddScoped<IUserManagerService, UserManagerService>();
            builder.Services.AddScoped<IInventoryManagerService, InventoryManagerService>();
            builder.Services.AddScoped<ISalesTransactionService, SalesTransactionService>();
            builder.Services.AddScoped<IPurchaseTransactionService, PurchaseTransactionService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Use the custom authentication middleware
            //app.UseMiddleware<AuthHandlerMiddleware>("DemoKey", app.Services);

            // Add authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

          

            app.Run();
        }
    }
}
