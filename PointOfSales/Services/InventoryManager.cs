using System;
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
        public static async Task AddProductAsync(int id, string name, decimal price, int quantity, string type, string category)
        {
            var newProduct = new Product
            {
                Id = id,
                Name = name,
                Price = price,
                Quantity = quantity,
                Type = type,
                Category = category
            };
            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();
        }

        // View all products
        public static IQueryable<Product> ViewProducts()
        {
            return _context.Products.AsQueryable();
        }

        // Update a product
        public static async Task<bool> UpdateProductAsync(int id, string name = "", decimal? price = null, int? quantity = null, string type = "", string category = "")
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(name)) product.Name = name;
                if (price.HasValue) product.Price = price.Value;
                if (quantity.HasValue) product.Quantity = quantity.Value;
                if (!string.IsNullOrEmpty(type)) product.Type = type;
                if (!string.IsNullOrEmpty(category)) product.Category = category;

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

        // Display inventory as a table
        public static async Task DisplayInventoryTableAsync()
        {
            var products = ViewProducts();
            Console.WriteLine($"Product Count: {products.Count()}");

            // Print the table header
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("| {0, -5} | {1, -20} | {2, -10} | {3, -15} | {4, -10} | {5, -10} |", "ID", "Name", "Price", "Category", "Quantity", "Type");
            Console.WriteLine("-------------------------------------------------------------------------------");

            // Print the table rows
            foreach (var item in products)
            {
                Console.WriteLine("| {0, -5} | {1, -20} | {2, -10:C} | {3, -15} | {4, -10} | {5, -10} |", item.Id, item.Name, item.Price, item.Category, item.Quantity, item.Type);
            }

            Console.WriteLine("-------------------------------------------------------------------------------");
        }

        public static async Task TrackInventoryAsync()
        {
            await DisplayInventoryTableAsync();
        }

        public static async Task<bool> ReceiveNewStockAsync(int id, int quantity)
        {
            var product = await FindProductByIDAsync(id);

            if (product != null)
            {
                product.Quantity += quantity;
                await UpdateProductAsync(product.Id, product.Name, product.Price, product.Quantity, product.Type);
                return true;
            }

            return false;
        }

        public static async Task<bool> ReduceStockAsync(int id, int quantity)
        {
            await TrackInventoryAsync();
            var product = await FindProductByIDAsync(id);

            if (product != null)
            {
                product.Quantity -= quantity;
                await UpdateProductAsync(product.Id, product.Name, product.Price, product.Quantity, product.Type);
                return true;
            }

            return false;
        }

        public static async Task ShowInventoryItemsAsync()
        {
            var productsInventory = ViewProducts().ToList();
            Console.WriteLine($"Products Count in Inventory: {productsInventory.Count}");
            Console.WriteLine();

            var receipt = new StringBuilder();
            receipt.AppendLine("Products in Inventory Receipt");
            receipt.AppendLine("-------------------------------");

            foreach (var item in productsInventory)
            {
                receipt.AppendLine($"{item.Name} : {item.Quantity}");
            }

            Console.WriteLine(receipt.ToString());
            Console.ReadKey();
        }
    }
}
