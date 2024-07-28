﻿using POS.API.MODEL.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.REPOSITORIES.PurchaseTransactionRepository
{
    public interface IPurchaseTransactionRepository
    {
        Task AddProductToPurchaseOrderAsync(int productId, int quantity);
        Task<decimal> CalculateTotalPurchaseAmountAsync();
        Task<PurchaseReceiptResponse> GeneratePurchaseReceiptInvoiceAsync();
        Task ClearPurchaseItemsAsync();
    }
}