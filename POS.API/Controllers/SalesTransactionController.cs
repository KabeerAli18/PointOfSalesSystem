using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POS.API.MODEL.Sales;
using POS.API.SERVICES.SaleServices;
using System;
using System.Threading.Tasks;
using WebApisPointOfSales.Dto;

namespace WebApisPointOfSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesTransactionController : ControllerBase
    {
        private readonly ISalesTransactionService _salesTransactionService;
        private readonly ILogger<SalesTransactionController> _logger;
        private readonly IMapper _mapper;

        public SalesTransactionController(ISalesTransactionService salesTransactionService, ILogger<SalesTransactionController> logger, IMapper mapper)
        {
            _salesTransactionService = salesTransactionService;
            _logger = logger;
            _mapper = mapper;
        }

        [Authorize(Policy = "RequireCashierRole")]
        [HttpPost("add-product-sales")]
        public async Task<ActionResult> AddProductToSale([FromQuery] SaleItemDTO itemDto)
        {
            try
            {
                var saleItem = _mapper.Map<SaleItem>(itemDto);
                _logger.LogInformation("Attempting to add product to sale. ProductId: {ProductId}, Quantity: {Quantity}", saleItem.ProductId, saleItem.Quantity);
                await _salesTransactionService.AddProductToSaleAsync(saleItem.ProductId, saleItem.Quantity);
                _logger.LogInformation("Product added to sale successfully. ProductId: {ProductId}, Quantity: {Quantity}", saleItem.ProductId, saleItem.Quantity);
                return Ok("Product added to sale successfully.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while adding product to sale. ProductId: {ProductId}, Quantity: {Quantity}", itemDto.ProductId, itemDto.Quantity);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "RequireCashierRole")]
        [HttpGet("total-sales-amount")]
        public async Task<ActionResult<decimal>> CalculateTotalSalesAmount()
        {
            try
            {
                _logger.LogInformation("Calculating total sales amount.");
                var totalAmount = await _salesTransactionService.CalculateTotalSalesAmountAsync();
                _logger.LogInformation("Total sales amount calculated successfully. TotalAmount: {TotalAmount}", totalAmount);
                return Ok(totalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating total sales amount.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while calculating the total sales amount.");
            }
        }

        [Authorize(Policy = "RequireCashierRole")]
        [HttpGet("sales-receipt")]
        public async Task<ActionResult<SalesReceiptResponse>> GenerateSalesTransactionsReceipt()
        {
            try
            {
                _logger.LogInformation("Generating sales transactions receipt.");
                var receipt = await _salesTransactionService.GenerateSalesTransactionsReceiptAsync();
                _logger.LogInformation("Sales transactions receipt generated successfully.");
                return Ok(receipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating sales transactions receipt.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while generating the sales receipt.");
            }
        }

        [Authorize(Policy = "RequireCashierRole")]
        [HttpDelete("clear-sales-History")]
        public async Task<ActionResult> ClearSaleItemsAsync()
        {
            try
            {
                _logger.LogInformation("Clearing all sale items.");
                await _salesTransactionService.ClearSaleItemsAsync();
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
