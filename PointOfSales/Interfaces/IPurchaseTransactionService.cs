using PointOfSales.Entities;
using System.Threading.Tasks;

namespace PointOfSales.Interfaces
{
    public interface IPurchaseTransactionService
    {
        Task AddProductToPurchaseOrderAsync(int productId, int quantity);
        Task<decimal> CalculateTotalPurchaseAmountAsync();
        Task<PurchaseReceiptResponse> GeneratePurchaseReceiptInvoiceAsync();
        Task ClearPurchaseItemsAsync();
    }
}
