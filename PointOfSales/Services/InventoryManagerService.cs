using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PointOfSales.Data;
using PointOfSales.Entities;
using PointOfSales.Interfaces;

namespace PointOfSales.Services
{
    public class InventoryManagerService : IInventoryManagerService
    {
        private readonly MyDbContext _context;

        // Constructor to initialize the DbContext
        public InventoryManagerService(MyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Add a new product
        public async Task<Product> AddProductAsync(Product product)
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
        public async Task<IEnumerable<Product>> TrackProductInventory()
        {
            return await _context.Products.ToListAsync();
        }

        // Update a product
        public async Task<bool> UpdateProductAsync(int id, Product producta)
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
        public async Task<bool> RemoveProductAsync(int id)
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

        public async Task<Product> ReceiveNewStockAsync(int id, int quantity)
        {
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

        public async Task<Product> ReduceStockAsync(int id, int quantity)
        {
            var product = await FindProductByIDAsync(id);

            if (product != null && quantity < product.Quantity && quantity > 0)
            {
                product.Quantity -= quantity;
                await UpdateProductAsync(id, product);
                return product;
            }
            else
            {
                throw new ArgumentException("Quantity of Product entered is not Present in stock");
            }
        }

        // Find a product by ID
        public async Task<Product> FindProductByIDAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new ArgumentException("Product not found.");
            }
            return product;
        }
    }
}
