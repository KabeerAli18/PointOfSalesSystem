using POS.API.MODEL.Sales;
using POS.API.REPOSITORIES.SalesTransactionRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.SERVICES.SaleServices
{
    public class SalesTransactionService : ISalesTransactionService
    {
        private readonly ISalesTransactionRepository _salesTransactionRepository;

        public SalesTransactionService(ISalesTransactionRepository salesTransactionRepository)
        {
            _salesTransactionRepository = salesTransactionRepository;
        }

        public async Task AddProductToSaleAsync(int productId, int quantity)
        {
            try
            {
                await _salesTransactionRepository.AddProductToSaleAsync(productId, quantity);
            }
            catch(ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding product to sale: ", ex);
            }
        }

        public async Task<decimal> CalculateTotalSalesAmountAsync()
        {
            try
            {
                return await _salesTransactionRepository.CalculateTotalSalesAmountAsync();
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating total sales amount: ", ex);
            }
        }

        public async Task<SalesReceiptResponse> GenerateSalesTransactionsReceiptAsync()
        {
            try
            {
                return await _salesTransactionRepository.GenerateSalesTransactionsReceiptAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while generating sales transaction receipt: ", ex);
            }
        }

        public async Task ClearSaleItemsAsync()
        {
            try
            {
                await _salesTransactionRepository.ClearSaleItemsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while clearing sale items: ", ex);
            }
        }
    }
}
