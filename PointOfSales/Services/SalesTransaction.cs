using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PointOfSales.Entities;

namespace PointOfSales.Services
{
    public static class SalesTransaction
    {
        private static MyDbContext _context = null!;


        // Method to initialize the DbContext
        public static void Initialize(MyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static async Task AddProductToSaleAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found in inventory.");
            }

            if (product.Quantity < quantity)
            {
                throw new InvalidOperationException("Insufficient quantity in stock.");
            }

            product.Quantity -= quantity;
            var saleItem = new SaleItem
            {
                ProductId = productId,
                Quantity = quantity
            };
            _context.SaleItems.Add(saleItem);
            await _context.SaveChangesAsync();
        }

        public static decimal CalculateTotalSalesAmount()
        {
            var saleItems = _context.SaleItems.Include(si => si.Product).ToList();
            return saleItems.Sum(item => item.Product.Price * item.Quantity);
        }

        public static string GenerateSalesTransactionsReceipt()
        {
            var saleItems = _context.SaleItems.Include(si => si.Product).ToList();
            var receipt = new StringBuilder();
            receipt.AppendLine("Sales Transaction Receipt");
            receipt.AppendLine("-------------------------------");

            foreach (var item in saleItems)
            {
                var itemTotalPrice = item.Product.Price * item.Quantity;
                receipt.AppendLine($"{item.Product.Name} x {item.Quantity} = {itemTotalPrice:C}");
            }

            receipt.AppendLine("-------------------------------");
            receipt.AppendLine($"Total: {CalculateTotalSalesAmount():C}");

            return receipt.ToString();
        }

        public static async Task ClearSaleItemsAsync()
        {
            _context.SaleItems.RemoveRange(_context.SaleItems);
            await _context.SaveChangesAsync();
        }

    }
}
