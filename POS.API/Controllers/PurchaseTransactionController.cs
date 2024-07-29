
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POS.API.MODEL.Purchase;
using POS.API.SERVICES.PurchaseServices;
using System;
using System.Threading.Tasks;
using WebApisPointOfSales.Dto;
// Alias for the conflicting namespace
using ProductsDtos = WebApisPointOfSales.Dto.ProductsDtos;

namespace WebApisPointOfSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // Require authentication for all actions in this controller
    public class PurchaseTransactionController : ControllerBase
    {
        private readonly IPurchaseTransactionService _purchaseTransactionService;
        private readonly ILogger<PurchaseTransactionController> _logger;
        private readonly IMapper _mapper;

        public PurchaseTransactionController(IPurchaseTransactionService purchaseTransactionService, ILogger<PurchaseTransactionController> logger, IMapper mapper)
        {
            _purchaseTransactionService = purchaseTransactionService ?? throw new ArgumentNullException(nameof(purchaseTransactionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        [Authorize(Policy = "RequireCashierRole")]
        [HttpPost("add-product-purchase")]
        public async Task<IActionResult> AddProductToPurchaseOrderAsync([FromQuery] PurchaseItemDto itemDto)
        {
            try
            {
                var product = _mapper.Map<PurchaseItem>(itemDto);
                _logger.LogInformation("Attempting to add product to purchase order. ProductId: {ProductId}, Quantity: {Quantity}", product.ProductId, itemDto.Quantity);

                await _purchaseTransactionService.AddProductToPurchaseOrderAsync(product.ProductId, itemDto.Quantity);

                _logger.LogInformation("Product added to purchase order successfully. ProductId: {ProductId}, Quantity: {Quantity}", product.ProductId, itemDto.Quantity);
                return Ok("Product added to purchase order successfully.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid operation exception occurred while adding product to purchase order. ProductId: {ProductId}, Quantity: {Quantity}", itemDto.ProductId, itemDto.Quantity);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding product to purchase order. ProductId: {ProductId}, Quantity: {Quantity}", itemDto.ProductId, itemDto.Quantity);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireCashierRole")]
        [HttpGet("total-purchase-amount")]
        public async Task<IActionResult> CalculateTotalPurchaseAmountAsync()
        {
            try
            {
                _logger.LogInformation("Calculating total purchase amount.");

                var totalAmount = await _purchaseTransactionService.CalculateTotalPurchaseAmountAsync();

                _logger.LogInformation("Total purchase amount calculated: {TotalAmount}", totalAmount);
                return Ok(totalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calculating total purchase amount.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireCashierRole")]
        [HttpGet("generate-purchase-receipt")]
        public async Task<IActionResult> GeneratePurchaseReceiptInvoiceAsync()
        {
            try
            {
                _logger.LogInformation("Generating purchase receipt invoice.");

                var receipt = await _purchaseTransactionService.GeneratePurchaseReceiptInvoiceAsync();

                _logger.LogInformation("Purchase receipt invoice generated successfully.");
                return Ok(receipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating purchase receipt invoice.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [Authorize(Policy = "RequireCashierRole")]
        [HttpDelete("clear-purchase-items-History")]
        public async Task<IActionResult> ClearPurchaseItemsAsync()
        {
            try
            {
                _logger.LogInformation("Clearing purchase items.");

                await _purchaseTransactionService.ClearPurchaseItemsAsync();

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
