using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PointOfSales;
using PointOfSales.Services;
using WebApisPointOfSales.MiddleWares;

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

            // Register any other services needed
            // builder.Services.AddTransient<YourService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Use the custom authentication middleware
            app.UseMiddleware<AuthHandler>("DemoKey", app.Services);

            // Add authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Initialize static classes with the DbContext
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                UserManager.Initialize(context);
                InventoryManager.Initialize(context);
                PurchaseTransactions.Initialize(context);
                SalesTransaction.Initialize(context);
            }

            app.Run();
        }
    }
}
