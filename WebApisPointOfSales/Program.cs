using Microsoft.EntityFrameworkCore;
using PointOfSales.Services;
using PointOfSales;

namespace PointOfSales
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            // Setup in-memory database context
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase("PointOfSalesDatabase")
                .Options;

            // Initialize DbContext
            using var context = new MyDbContext(options);
            // Initialize static classes with the DbContext
            UserManager.Initialize(context);
            InventoryManager.Initialize(context);
            PurchaseTransactions.Initialize(context);
            SalesTransaction.Initialize(context);
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}