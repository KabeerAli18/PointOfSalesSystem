using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PointOfSales.Entities;
using PointOfSales.Services;

namespace PointOfSales
{
    class Program
    {
        static async Task Main(string[] args)
        {
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

            // Example usage of the static classes

            // Register a new user
            try
            {
                //UserManager.RegisterUser("John Doe", "john.doe@example.com", "SecurePassword123", "Cashier");
                Console.WriteLine("User registered successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
            }

            // Log in a user
            try
            {
                var user = UserManager.LogInUserAuthentication("john.doe@example.com", "SecurePassword123");
               // Console.WriteLine($"User logged in: {user.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging in user: {ex.Message}");
            }

            // Add a product to inventory
            try
            {
                //await InventoryManager.AddProductAsync(1, "Product A", 10.99m, 100, "Electronics", "Gadgets");
                Console.WriteLine("Product added to inventory.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
            }

            // Display inventory
           // await InventoryManager.ShowInventoryItemsAsync();

            // Add a product to purchase order
            try
            {
                await PurchaseTransactions.AddProductToPurchaseOrderAsync(1, 10);
                Console.WriteLine("Product added to purchase order.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product to purchase order: {ex.Message}");
            }

            // Generate purchase receipt
            Console.WriteLine(PurchaseTransactions.GeneratePurchaseReceiptInvoice());

            // Add a product to sale
            try
            {
                await SalesTransaction.AddProductToSaleAsync(1, 5);
                Console.WriteLine("Product added to sale.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product to sale: {ex.Message}");
            }

            // Generate sales transaction receipt
            Console.WriteLine(SalesTransaction.GenerateSalesTransactionsReceipt());

            // Clear purchase items
            try
            {
                PurchaseTransactions.ClearPurchaseItems();
                Console.WriteLine("Purchase items cleared.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing purchase items: {ex.Message}");
            }

            // Clear sale items
            try
            {
                await SalesTransaction.ClearSaleItemsAsync();
                Console.WriteLine("Sale items cleared.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing sale items: {ex.Message}");
            }
        }
    }
}
