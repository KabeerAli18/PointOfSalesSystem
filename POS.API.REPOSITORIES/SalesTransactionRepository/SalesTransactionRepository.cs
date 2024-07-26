using Microsoft.EntityFrameworkCore;
using POS.API.DATA;
using POS.API.MODEL.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.REPOSITORIES.SalesTransactionRepository
{
    public class SalesTransactionRepository : ISalesTransactionRepository
    {
        private readonly MyDbContext _context;

        public SalesTransactionRepository(MyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddProductToSaleAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new ArgumentException("Product not found in inventory.");
            }

            if (product.Quantity < quantity)
            {
                throw new ArgumentException("Insufficient quantity in stock.");
            }

            product.Quantity -= quantity;
            var saleItem = new SaleItem
            {
                ProductId = productId,
                Quantity = quantity,
                Price = product.Price,
                SalesItemName = product.Name,
                Product = product
            };
            _context.SaleItems.Add(saleItem);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> CalculateTotalSalesAmountAsync()
        {
            var saleItems = await _context.SaleItems.Include(si => si.Product).ToListAsync();
            return saleItems.Sum(item => item.Price * item.Quantity);
        }

        public async Task<SalesReceiptResponse> GenerateSalesTransactionsReceiptAsync()
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

        public async Task ClearSaleItemsAsync()
        {
            _context.SaleItems.RemoveRange(_context.SaleItems);
            await _context.SaveChangesAsync();
        }
    }
}
