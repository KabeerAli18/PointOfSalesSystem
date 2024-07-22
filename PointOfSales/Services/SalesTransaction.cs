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
                Quantity = quantity,
                Price = product.Price, // Set the price from the product
                SalesItemName = product.Name,
                Product = product // Reference the existing product
            };
            _context.SaleItems.Add(saleItem);
            await _context.SaveChangesAsync();
        }

        public static async Task<decimal> CalculateTotalSalesAmountAsync()
        {
            var saleItems = await _context.SaleItems.Include(si => si.Product).ToListAsync();
            return saleItems.Sum(item => item.Price * item.Quantity);
        }

        public static async Task<SalesReceiptResponse> GenerateSalesTransactionsReceiptAsync()
        {
            var saleItems = await _context.SaleItems.Include(si => si.Product).ToListAsync();
            var receiptItems = saleItems.Select(item => new SaleItemResponse
            {
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList();

            return new SalesReceiptResponse
            {
                ReceiptHeader = "Sales Transaction Receipt/Invoice",
                SaleItems = receiptItems,
                TotalAmount = await CalculateTotalSalesAmountAsync()
            };
        }


        public static async Task ClearSaleItemsAsync()
        {
            _context.SaleItems.RemoveRange(_context.SaleItems);
            await _context.SaveChangesAsync();
        }
    }
}
