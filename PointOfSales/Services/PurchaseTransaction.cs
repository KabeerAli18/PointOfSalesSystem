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
            if (product.Quantity < quantity)
            {
                throw new InvalidOperationException("Insufficient quantity in stock.");
            }

            product.Quantity += quantity; //Increase The Product
            var purchaseItem = new PurchaseItem
            {
                ProductId = productId,
                Quantity = quantity,
                Price = product.Price, // Set the price from the product
                PurchaseItemName = product.Name,
                Product = product // Reference the existing product
            };
            _context.PurchaseItems.Add(purchaseItem);
            await _context.SaveChangesAsync();
        }

        public static async Task<decimal> CalculateTotalPurchaseAmountAsync()
        {
            var purchaseItems = await _context.PurchaseItems.Include(pi => pi.Product).ToListAsync();
            return purchaseItems.Sum(item => item.Product.Price * item.Quantity);
        }

        public static async Task<PurchaseReceiptResponse> GeneratePurchaseReceiptInvoiceAsync()
        {
            var purchaseItems = await _context.PurchaseItems.Include(pi => pi.Product).ToListAsync();
            var receiptItems = purchaseItems.Select(item => new PurchaseItemResponse
            {
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList();

            return new PurchaseReceiptResponse
            {
                ReceiptHeader = "Purchase Receipt/Invoice",
                PurchaseItems = receiptItems,
                TotalAmount = await CalculateTotalPurchaseAmountAsync()
            };
        }

        public static async Task ClearPurchaseItemsAsync()
        {
            _context.PurchaseItems.RemoveRange(_context.PurchaseItems);
            await _context.SaveChangesAsync();
        }
    }

    
}
