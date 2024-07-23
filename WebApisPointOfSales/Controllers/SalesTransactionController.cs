using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;  // Import the ILogger namespace
using PointOfSales;
using PointOfSales.Entities;
using PointOfSales.Services;

namespace WebApisPointOfSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SalesTransactionController : ControllerBase
    {
        private readonly ILogger<SalesTransactionController> _logger;  // Add ILogger field

        public SalesTransactionController(MyDbContext myDbContext, ILogger<SalesTransactionController> logger)
        {
            SalesTransaction.Initialize(myDbContext);
            _logger = logger;  // Initialize the logger
        }

        [HttpPost("add-product-sales")]
        public async Task<ActionResult> AddProductToSale([FromQuery] int productId, [FromQuery] int quantity)
        {
            try
            {
                _logger.LogInformation("Attempting to add product to sale. ProductId: {ProductId}, Quantity: {Quantity}", productId, quantity);
                await SalesTransaction.AddProductToSaleAsync(productId, quantity);
                _logger.LogInformation("Product added to sale successfully. ProductId: {ProductId}, Quantity: {Quantity}", productId, quantity);
                return Ok("Product added to sale successfully.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error occurred while adding product to sale. ProductId: {ProductId}, Quantity: {Quantity}", productId, quantity);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("total-sales-amount")]
        public async Task<ActionResult<decimal>> CalculateTotalSalesAmount()
        {
            try
            {
                _logger.LogInformation("Calculating total sales amount.");
                var totalAmount = await SalesTransaction.CalculateTotalSalesAmountAsync();
                _logger.LogInformation("Total sales amount calculated successfully. TotalAmount: {TotalAmount}", totalAmount);
                return Ok(totalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating total sales amount.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while calculating the total sales amount.");
            }
        }

        [HttpGet("sales-receipt")]
        public async Task<ActionResult<SalesReceiptResponse>> GenerateSalesTransactionsReceipt()
        {
            try
            {
                _logger.LogInformation("Generating sales transactions receipt.");
                var receipt = await SalesTransaction.GenerateSalesTransactionsReceiptAsync();
                _logger.LogInformation("Sales transactions receipt generated successfully.");
                return Ok(receipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating sales transactions receipt.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while generating the sales receipt.");
            }
        }

        [HttpDelete("clear-sales")]
        public async Task<ActionResult> ClearSaleItemsAsync()
        {
            try
            {
                _logger.LogInformation("Clearing all sale items.");
                await SalesTransaction.ClearSaleItemsAsync();
                _logger.LogInformation("All sale items have been cleared.");
                return Ok("All sale items have been cleared.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while clearing sale items.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while clearing sale items.");
            }
        }
    }
}
