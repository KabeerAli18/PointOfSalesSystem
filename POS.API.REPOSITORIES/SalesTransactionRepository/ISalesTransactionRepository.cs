using POS.API.MODEL.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.REPOSITORIES.SalesTransactionRepository
{
    public interface ISalesTransactionRepository
    {
        Task AddProductToSaleAsync(int productId, int quantity);
        Task<decimal> CalculateTotalSalesAmountAsync();
        Task<SalesReceiptResponse> GenerateSalesTransactionsReceiptAsync();
        Task ClearSaleItemsAsync();
    }
}
