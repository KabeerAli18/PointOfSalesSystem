using POS.API.MODEL.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.SERVICES.SaleServices
{
    public interface ISalesTransactionService
    {
        Task AddProductToSaleAsync(string productId, int quantity);
        Task<decimal> CalculateTotalSalesAmountAsync();
        Task<SalesReceiptResponse> GenerateSalesTransactionsReceiptAsync();
        Task ClearSaleItemsAsync();
    }
}
