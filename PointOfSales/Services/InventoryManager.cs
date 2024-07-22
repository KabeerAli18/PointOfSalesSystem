using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PointOfSales.Entities;

namespace PointOfSales.Services
{
    public static class InventoryManager 
    {
        private static MyDbContext _context = null!;

        // Method to initialize the DbContext
        public static void Initialize(MyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Add a new product
        public static async Task<Product> AddProductAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product is null.");
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new ArgumentException("Product name is required.", nameof(product.Name));
            }

            if (product.Quantity < 0)
            {
                throw new ArgumentException("Product quantity cannot be negative.", nameof(product.Quantity));
            }

            if (product.Price < 0)
            {
                throw new ArgumentException("Product price cannot be negative.", nameof(product.Price));
            }

            if (string.IsNullOrWhiteSpace(product.Type))
            {
                throw new ArgumentException("Product type is required.", nameof(product.Type));
            }
            if (string.IsNullOrWhiteSpace(product.Category))
            {
                throw new ArgumentException("Product Category is required.", nameof(product.Category));
            }

            // Check for duplicate product by name and type
            bool productExists = await _context.Products
                .AnyAsync(p => p.Name == product.Name && p.Type == product.Type);

            if (productExists)
            {
                throw new InvalidOperationException("A product with the same name and type already exists.");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        // View all products

        public static async Task<IEnumerable<Product>> TrackProductInventory()
        {
            return await _context.Products.ToListAsync();
        }
        // Update a product
        public static async Task<bool> UpdateProductAsync(int id, Product producta)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // No need to check for nullable types if they are not nullable
                product.Id = id;
                if (!string.IsNullOrEmpty(producta.Name)) product.Name = producta.Name;
                product.Price = producta.Price; // No HasValue check needed for non-nullable types
                product.Quantity = producta.Quantity; // No HasValue check needed for non-nullable types
                if (!string.IsNullOrEmpty(producta.Type)) product.Type = producta.Type;
                if (!string.IsNullOrEmpty(producta.Category)) product.Category = producta.Category;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        // Remove a product
        public static async Task<bool> RemoveProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public static async Task<Product> ReceiveNewStockAsync(int id, int quantity)
        {

            // await TrackInventoryAsync();
            var product = await FindProductByIDAsync(id);

            if (product != null && quantity > 0)
            {
                product.Quantity += quantity;
                await UpdateProductAsync(id, product);
                return product;
            }
            else
            {
                throw new ArgumentException("Quantity of Product entered is not Present in stock");
            }
        }

        public static async Task<Product> ReduceStockAsync(int id, int quantity)
        {
            // await TrackInventoryAsync();
            var product = await FindProductByIDAsync(id);

            if (product != null && quantity<product.Quantity && quantity>0)
            {
                product.Quantity -= quantity;
                await UpdateProductAsync(id,product);
                return product;
            }
            else
            {
                throw new ArgumentException("Quantity of Product entered is not Present in stock");
            }

            
        }
        // Find a product by ID
        public static async Task<Product> FindProductByIDAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new ArgumentException("Product not found.");
            }
            return product;
        }

        //// Display inventory as a table
        //public static async Task DisplayInventoryTableAsync()
        //{
        //    var products = ViewProducts();
        //    Console.WriteLine($"Product Count: {products.Count()}");

        //    // Print the table header
        //    Console.WriteLine("-------------------------------------------------------------------------------");
        //    Console.WriteLine("| {0, -5} | {1, -20} | {2, -10} | {3, -15} | {4, -10} | {5, -10} |", "ID", "Name", "Price", "Category", "Quantity", "Type");
        //    Console.WriteLine("-------------------------------------------------------------------------------");

        //    // Print the table rows
        //    foreach (var item in products)
        //    {
        //        Console.WriteLine("| {0, -5} | {1, -20} | {2, -10:C} | {3, -15} | {4, -10} | {5, -10} |", item.Id, item.Name, item.Price, item.Category, item.Quantity, item.Type);
        //    }

        //    Console.WriteLine("-------------------------------------------------------------------------------");
        //}

        //public static async Task TrackInventoryAsync()
        //{
        //    await DisplayInventoryTableAsync();
        //}


        //public static async Task ShowInventoryItemsAsync()
        //{
        //    var productsInventory = ViewProducts().ToList();
        //    Console.WriteLine($"Products Count in Inventory: {productsInventory.Count}");
        //    Console.WriteLine();

        //    var receipt = new StringBuilder();
        //    receipt.AppendLine("Products in Inventory Receipt");
        //    receipt.AppendLine("-------------------------------");

        //    foreach (var item in productsInventory)
        //    {
        //        receipt.AppendLine($"{item.Name} : {item.Quantity}");
        //    }

        //    Console.WriteLine(receipt.ToString());
        //    Console.ReadKey();
        //}
    }
}
