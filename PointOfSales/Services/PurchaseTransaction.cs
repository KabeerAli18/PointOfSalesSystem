using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PointOfSales.Entities;

namespace PointOfSales.Services
{
    public static class PurchaseTransactions
    {
        private static MyDbContext _context = null!;

        // Method to initialize the DbContext
        public static void Initialize(MyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static async Task AddProductToPurchaseOrderAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found in inventory.");
            }

            product.Quantity += quantity;
            var purchaseItem = new PurchaseItem
            {
                ProductId = productId,
                Quantity = quantity
            };
            _context.PurchaseItems.Add(purchaseItem);
            await _context.SaveChangesAsync();
        }

        public static decimal CalculateTotalPurchaseAmount()
        {
            var purchaseItems = _context.PurchaseItems.Include(pi => pi.Product).ToList();
            return purchaseItems.Sum(item => item.Product.Price * item.Quantity);
        }

        public static string GeneratePurchaseReceiptInvoice()
        {
            var purchaseItems = _context.PurchaseItems.Include(pi => pi.Product).ToList();
            var receipt = new StringBuilder();
            receipt.AppendLine("Purchase Receipt");
            receipt.AppendLine("-------------------------------");

            foreach (var item in purchaseItems)
            {
                var itemTotalPrice = item.Product.Price * item.Quantity;
                receipt.AppendLine($"{item.Product.Name} x {item.Quantity} = {itemTotalPrice:C}");
            }

            receipt.AppendLine("-------------------------------");
            receipt.AppendLine($"Total: {CalculateTotalPurchaseAmount():C}");

            return receipt.ToString();
        }

        public static void ClearPurchaseItems()
        {
            _context.PurchaseItems.RemoveRange(_context.PurchaseItems);
            _context.SaveChanges();
        }
    }
}
