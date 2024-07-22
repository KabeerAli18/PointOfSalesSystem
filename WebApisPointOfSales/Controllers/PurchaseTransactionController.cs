using Microsoft.AspNetCore.Mvc;
using PointOfSales;
using PointOfSales.Entities;
using PointOfSales.Services;
using System;
using System.Threading.Tasks;

namespace WebApisPointOfSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseTransactionController : ControllerBase
    {
        public PurchaseTransactionController(MyDbContext context)
        {
            PurchaseTransactions.Initialize(context);
        }

        [HttpPost("add-product-purchase")]
        public async Task<IActionResult> AddProductToPurchaseOrderAsync(int productId, int quantity)
        {
            try
            {
                await PurchaseTransactions.AddProductToPurchaseOrderAsync(productId, quantity);
                return Ok("Product added to purchase order successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("total-purchase-amount")]
        public async Task<IActionResult> CalculateTotalPurchaseAmountAsync()
        {
            try
            {
                var totalAmount = await PurchaseTransactions.CalculateTotalPurchaseAmountAsync();
                return Ok(totalAmount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("generate-purchase-receipt")]
        public async Task<IActionResult> GeneratePurchaseReceiptInvoiceAsync()
        {
            try
            {
                var receipt = await PurchaseTransactions.GeneratePurchaseReceiptInvoiceAsync();
                return Ok(receipt);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpDelete("clear-purchase-items")]
        public async Task<IActionResult> ClearPurchaseItemsAsync()
        {
            try
            {
                await PurchaseTransactions.ClearPurchaseItemsAsync();
                return Ok("Purchase items cleared successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
