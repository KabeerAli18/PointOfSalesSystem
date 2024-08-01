using POS.API.MODEL.Purchase;
using POS.API.REPOSITORIES.PurchaseTransactionRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.SERVICES.PurchaseServices
{
    public class PurchaseTransactionServices : IPurchaseTransactionService
    {
        private readonly IPurchaseTransactionRepository _purchaseTransactionRepository;

        public PurchaseTransactionServices(IPurchaseTransactionRepository purchaseTransactionRepository)
        {
            _purchaseTransactionRepository = purchaseTransactionRepository;
        }

        public async Task AddProductToPurchaseOrderAsync(string productId, int quantity)
        {
            try
            {
                await _purchaseTransactionRepository.AddProductToPurchaseOrderAsync(productId, quantity);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding product to purchase order: ", ex);
            }
        }

        public async Task<decimal> CalculateTotalPurchaseAmountAsync()
        {
            try
            {
                return await _purchaseTransactionRepository.CalculateTotalPurchaseAmountAsync();
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating total purchase amount: ", ex);
            }
        }

        public async Task<PurchaseReceiptResponse> GeneratePurchaseReceiptInvoiceAsync()
        {
            try
            {
                return await _purchaseTransactionRepository.GeneratePurchaseReceiptInvoiceAsync();
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while generating purchase receipt invoice: ", ex);
            }
        }

        public async Task ClearPurchaseItemsAsync()
        {
            try
            {
                await _purchaseTransactionRepository.ClearPurchaseItemsAsync();
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while clearing purchase items: ", ex);
            }
        }
    }
}
