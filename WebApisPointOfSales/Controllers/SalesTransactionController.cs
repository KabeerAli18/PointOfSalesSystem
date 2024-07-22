using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PointOfSales;
using PointOfSales.Entities;
using PointOfSales.Services;

namespace WebApisPointOfSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesTransactionController : ControllerBase
    {
        public SalesTransactionController(MyDbContext myDbContext)
        {
            SalesTransaction.Initialize(myDbContext);
        }

        [HttpPost("add-product-sales")]
        public async Task<ActionResult> AddProductToSale([FromQuery] int productId, [FromQuery] int quantity)
        {
            try
            {
                await SalesTransaction.AddProductToSaleAsync(productId, quantity);
                return Ok("Product added to sale successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("total-sales-amount")]
        public async Task<ActionResult<decimal>> CalculateTotalSalesAmount()
        {
            var totalAmount = await SalesTransaction.CalculateTotalSalesAmountAsync();
            return Ok(totalAmount);
        }

        [HttpGet("sales-receipt")]
        public async Task<ActionResult<SalesReceiptResponse>> GenerateSalesTransactionsReceipt()
        {
            var receipt = await SalesTransaction.GenerateSalesTransactionsReceiptAsync();
            return Ok(receipt);
        }
        [HttpDelete("clear-sales")]
        public async Task<ActionResult> ClearSaleItemsAsync()
        {
            await SalesTransaction.ClearSaleItemsAsync();
            return Ok("All sale items have been cleared.");
        }
    }
}
