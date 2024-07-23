using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PointOfSales.Services;
using PointOfSales;
using WebApisPointOfSales.MiddleWares;

namespace PointOfSales
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register MyDbContext with the DI container
            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseInMemoryDatabase("PointOfSalesDatabase"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
