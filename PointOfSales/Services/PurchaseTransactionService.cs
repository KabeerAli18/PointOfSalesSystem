﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PointOfSales.Data;
using PointOfSales.Entities;
using PointOfSales.Interfaces;

namespace PointOfSales.Services
{
    public class PurchaseTransactionService : IPurchaseTransactionService
    {
        private readonly MyDbContext _context;

        public PurchaseTransactionService(MyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddProductToPurchaseOrderAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found in inventory.");
            }

            product.Quantity += quantity; // Increase the product quantity
            var purchaseItem = new PurchaseItem
            {
                ProductId = productId,
                Quantity = quantity,
                Price = product.Price,
                PurchaseItemName = product.Name,
                Product = product
            };
            _context.PurchaseItems.Add(purchaseItem);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> CalculateTotalPurchaseAmountAsync()
        {
            var purchaseItems = await _context.PurchaseItems.Include(pi => pi.Product).ToListAsync();
            return purchaseItems.Sum(item => item.Product.Price * item.Quantity);
        }

        public async Task<PurchaseReceiptResponse> GeneratePurchaseReceiptInvoiceAsync()
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

        public async Task ClearPurchaseItemsAsync()
        {
            _context.PurchaseItems.RemoveRange(_context.PurchaseItems);
            await _context.SaveChangesAsync();
        }
    }
}