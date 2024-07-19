using System;
using System.Linq;
using System.Text;

namespace POS
{
    public static class InventoryManager
    {
        private static List<Product> productsInventory = new List<Product> {
            new Product(1, "Milk", 19.99m, 10, "Book", "Fiction"),
            new Product(2, "Balls", 29.99m, 5, "Cricket", "Non-Fiction"),
            new Product(3, "Antique Book", 99.99m, 2, "Book", "Rare"),
            new Product(4, "Book C", 15.99m, 20, "Book", "Children"),
            new Product(5, "Magazine A", 5.99m, 30, "Magazine", "Lifestyle")
        };

        // Add a new product
        public static void AddProduct(int id, string name, decimal price, int quantity, string type, string category)
        {
            var newProduct = new Product(id, name, price, quantity, type, category);
            productsInventory.Add(newProduct);
        }

        // View all products
        public static List<Product> ViewProducts()
        {
            return productsInventory;
        }

        // Update a product
        public static bool UpdateProduct(int id, string name = "", decimal? price = null, int? quantity = null, string type = "", string category = "")
        {
            var product = productsInventory.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(name)) product.Name = name;
                if (price.HasValue) product.Price = price.Value;
                if (quantity.HasValue) product.Quantity = quantity.Value;
                if (!string.IsNullOrEmpty(type)) product.Type = type;
                if (!string.IsNullOrEmpty(category)) product.Category = category;
                return true;
            }
            return false;
        }

        // Remove a product
        public static bool RemoveProduct(int id)
        {
            var product = productsInventory.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                productsInventory.Remove(product);
                return true;
            }
            return false;
        }

        // Find a product by ID
        public static Product FindProductByID(int id)
        {
            var product = productsInventory.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                throw new ArgumentException("Product not found.");
            }
            return product;
        }


        // Display inventory as a table
        public static void DisplayInventoryTable()
        {
            var products = ViewProducts();
            Console.WriteLine($"Product Count: {products.Count}");

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
        public static void TrackInventory()
        {
            Console.WriteLine("Current Inventory of Products:");
            var products = ViewProducts();
            Console.WriteLine($"Product Count: {products.Count}");

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

        public static bool ReceiveNewStock(int id, int quantity)
        {
            var product = ViewProducts().FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                product.Quantity += quantity;
                InventoryManager.UpdateProduct(product.Id, product.Name, product.Price, product.Quantity,product.Type);
                return true;
            }

            return false;
        }

        public static bool ReduceStock(int id, int quantity)
        {
            TrackInventory();
            var product = ViewProducts().FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                product.Quantity -= quantity;
                InventoryManager.UpdateProduct(product.Id, product.Name, product.Price, product.Quantity, product.Type);
                return true;
            }

            return false;
        }

        public static void ShowInventoryItems()
        {
            var productsInventory = ViewProducts();
            Console.WriteLine($"Products Count in Inventory: {productsInventory.Count()}");
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
