using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSales;
using PointOfSales.Entities;
using PointOfSales.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebApisPointOfSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseTransactionController : ControllerBase
    {
        private readonly ILogger<PurchaseTransactionController> _logger;

        public PurchaseTransactionController(MyDbContext context, ILogger<PurchaseTransactionController> logger)
        {
            PurchaseTransactions.Initialize(context);
            _logger = logger;
        }

        [HttpPost("add-product-purchase")]
        public async Task<IActionResult> AddProductToPurchaseOrderAsync([FromQuery] int productId, [FromQuery] int quantity)
        {
            try
            {
                _logger.LogInformation("Attempting to add product to purchase order. ProductId: {ProductId}, Quantity: {Quantity}", productId, quantity);

                await PurchaseTransactions.AddProductToPurchaseOrderAsync(productId, quantity);

                _logger.LogInformation("Product added to purchase order successfully. ProductId: {ProductId}, Quantity: {Quantity}", productId, quantity);
                return Ok("Product added to purchase order successfully.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation exception occurred while adding product to purchase order. ProductId: {ProductId}, Quantity: {Quantity}", productId, quantity);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding product to purchase order. ProductId: {ProductId}, Quantity: {Quantity}", productId, quantity);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("total-purchase-amount")]
        public async Task<IActionResult> CalculateTotalPurchaseAmountAsync()
        {
            try
            {
                _logger.LogInformation("Calculating total purchase amount.");

                var totalAmount = await PurchaseTransactions.CalculateTotalPurchaseAmountAsync();

                _logger.LogInformation("Total purchase amount calculated: {TotalAmount}", totalAmount);
                return Ok(totalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calculating total purchase amount.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("generate-purchase-receipt")]
        public async Task<IActionResult> GeneratePurchaseReceiptInvoiceAsync()
        {
            try
            {
                _logger.LogInformation("Generating purchase receipt invoice.");

                var receipt = await PurchaseTransactions.GeneratePurchaseReceiptInvoiceAsync();

                _logger.LogInformation("Purchase receipt invoice generated successfully.");
                return Ok(receipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating purchase receipt invoice.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpDelete("clear-purchase-items")]
        public async Task<IActionResult> ClearPurchaseItemsAsync()
        {
            try
            {
                _logger.LogInformation("Clearing purchase items.");

                await PurchaseTransactions.ClearPurchaseItemsAsync();

                _logger.LogInformation("Purchase items cleared successfully.");
                return Ok("Purchase items cleared successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while clearing purchase items.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
