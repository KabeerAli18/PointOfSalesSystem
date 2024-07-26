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
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.OAuth;
using POS.API.AutoMapper;
using POS.API.DATA;
using POS.API.MiddleWares;
using POS.API.SERVICES.UserServices;
using POS.API.REPOSITORIES.UsersRepository;
using POS.API.REPOSITORIES.ProductRepository;
using POS.API.SERVICES.ProductServices;
using POS.API.REPOSITORIES.SalesTransactionRepository;
using POS.API.SERVICES.SaleServices;
using POS.API.REPOSITORIES.PurchaseTransactionRepository;
using POS.API.SERVICES.PurchaseServices;

namespace POS.API
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

            // Register services and repositories for UsersManagement
            builder.Services.AddScoped<IUserManagerRepository, UserManagerRepository>();
            builder.Services.AddScoped<IUserManagerService, UserManagerService>();

            // Register services and repositories for Inventory Management
            builder.Services.AddScoped<IInventoryManagerRepository, InventoryManagerRepository>();
            builder.Services.AddScoped<IInventoryManagerService, InventoryManagerService>();

            // Register services and repositories for Sales Transactions
            builder.Services.AddScoped<ISalesTransactionRepository, SalesTransactionRepository>();
            builder.Services.AddScoped<ISalesTransactionService, SalesTransactionService>();

            // Register services and repositories for Purchase Transactions
            builder.Services.AddScoped<IPurchaseTransactionRepository, PurchaseTransactionRepository>();
            builder.Services.AddScoped<IPurchaseTransactionService, PurchaseTransactionServices>();

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
