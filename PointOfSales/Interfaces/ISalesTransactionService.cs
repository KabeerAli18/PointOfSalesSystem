using PointOfSales.Entities;
using System.Threading.Tasks;

namespace PointOfSales.Interfaces
{
    public interface ISalesTransactionService
    {
        Task AddProductToSaleAsync(int productId, int quantity);
        Task<decimal> CalculateTotalSalesAmountAsync();
        Task<SalesReceiptResponse> GenerateSalesTransactionsReceiptAsync();
        Task ClearSaleItemsAsync();
    }
}
