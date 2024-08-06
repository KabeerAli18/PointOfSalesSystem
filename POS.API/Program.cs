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
using Microsoft.Azure.Cosmos;
using System.Configuration;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Identity.Web;

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

            //// Register MyDbContext with the DI container
            //builder.Services.AddDbContext<MyDbContext>(options =>
            //    options.UseInMemoryDatabase("PointOfSalesDatabase"));


            // Add Azure Key Vault
            var keyVaultName = builder.Configuration["KeyVaultName"];
            var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
            builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

            //var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
            //logger.LogInformation($"CosmosDB Account: {keyVaultName}");
            //logger.LogInformation($"CosmosDB Key: {keyVaultUri}");

            // Configuration of Cosmos DB
            var cosmosDbSettings = builder.Configuration.GetSection("CosmosDb");
            var account = cosmosDbSettings["Account"];
            var key1 = cosmosDbSettings["Key"];

            // Validate that the key is a valid Base-64 string
            try
            {
                Convert.FromBase64String(key1);
            }
            catch (FormatException)
            {
                throw new ArgumentException("The Cosmos DB key is not a valid Base-64 string.");
            }

            builder.Services.AddSingleton(serviceProvider => new CosmosClient(account, key1));


            //Configurations of Cosmos Based Repositories
            builder.Services.AddScoped<IUserManagerRepository>(serviceProvider =>
            {
                var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
                return new UserManagerCosmosRepository(cosmosClient, cosmosDbSettings["DatabaseName"], "Users");
            });

            //For Products Inventory
            builder.Services.AddScoped<IInventoryManagerRepository>(serviceProvider =>
            {
                var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
                return new InventoryManagerCosmosRepository(cosmosClient, cosmosDbSettings["DatabaseName"], "Products");
            });

            //For Sales Transactions
            builder.Services.AddScoped<ISalesTransactionRepository>(serviceProvider =>
            {
                var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
                return new SalesTransactionCosmosRepository(cosmosClient, cosmosDbSettings["DatabaseName"], "SaleItems", "Products");
            });

            //For Purchase Transactions
            //For Sales Transactions
            builder.Services.AddScoped<IPurchaseTransactionRepository>(serviceProvider =>
            {
                var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
                return new PurchaseTransactionCosmosRepository(cosmosClient, cosmosDbSettings["DatabaseName"], "PurchaseItems", "Products");
            });

            // Configure Azure AD authentication
            //var azureAdSettings = builder.Configuration.GetSection("AzureAd");
            //builder.Services.AddAuthentication()
            //    .AddMicrosoftIdentityWebApi(azureAdSettings);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddMicrosoftIdentityWebApi(jwtBrearerOptions => {
                   jwtBrearerOptions.TokenValidationParameters.ValidateIssuer = true;
                   jwtBrearerOptions.TokenValidationParameters.ValidAudience = "api://44851630-fa2f-4a69-b3bf-1a4c8086eb37";
               },
               microsoftindentityoptions =>
               {
                   builder.Configuration.GetSection("AzureAD").Bind(microsoftindentityoptions);
               }, "Bearer", true);

            // Configure JWT authentication
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSettings.GetValue<string>("Key") ?? throw new ArgumentNullException(nameof(jwtSettings));
            var jwtIssuer = jwtSettings.GetValue<string>("Issuer") ?? throw new ArgumentNullException(nameof(jwtSettings));
            var jwtAudience = jwtSettings.GetValue<string>("Audience") ?? throw new ArgumentNullException(nameof(jwtSettings));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Roles";
                options.DefaultChallengeScheme = "Roles";
            })
            .AddJwtBearer("Roles", options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            // Add authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireCashierRole", policy => policy.RequireRole("Cashier"));
                options.AddPolicy("RequireAdminOrCashierRole", policy => policy.RequireRole("Admin","Cashier"));
            });


            //MIDDLE WARES
            // Register AuthService MiddleWares
            builder.Services.AddSingleton(new AuthBearerMiddleware(jwtKey, jwtIssuer, jwtAudience));

            //Mapper Configuration
            builder.Services.AddAutoMapper(typeof(MappingProfile)); // Add AutoMapper


            //Services

            builder.Services.AddScoped<IUserManagerService, UserManagerService>();
            //// Register services for Inventory Management
            builder.Services.AddScoped<IInventoryManagerService, InventoryManagerService>();
            //// Register services for Sales Transactions
            builder.Services.AddScoped<ISalesTransactionService, SalesTransactionService>();
            //// Register services for Purchase Transactions
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
           app.UseMiddleware<CustomExceptionHandlerMiddleware>();

            // Add authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();



            app.Run();
        }
    }
}
