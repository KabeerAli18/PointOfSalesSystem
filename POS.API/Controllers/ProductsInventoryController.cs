using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using AutoMapper;
using WebApisPointOfSales.Dto.ProductsDtos;
using POS.API.SERVICES.ProductServices;
using POS.API.MODEL.Products;
using Microsoft.Identity.Web.Resource;
using Microsoft.AspNetCore.Authentication.JwtBearer;

/// <summary>
/// This is The Product Inventory Controller, Holding end Points for inventory Apis like CRUD and Adding and receiving new Stocks
/// </summary>

namespace WebApisPointOfSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    //[Authorize("Admin")] // Only Admin Can do this.
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [RequiredScope("Forecast.Read")]

    public class ProductsInventoryController : ControllerBase
    {
        private readonly ILogger<ProductsInventoryController> _logger;
        private readonly IInventoryManagerService _inventoryManagerService;
        private readonly IMapper _mapper;

        public ProductsInventoryController(IInventoryManagerService inventoryManagerService, ILogger<ProductsInventoryController> logger, IMapper mapper)
        {
            _inventoryManagerService = inventoryManagerService ?? throw new ArgumentNullException(nameof(inventoryManagerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("add")]
       [Authorize(AuthenticationSchemes = "Roles", Policy = "RequireAdminRole")]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                _logger.LogInformation("Attempting to add a new product with name: {ProductName}", product.Name);
                var addedProduct = await _inventoryManagerService.AddProductAsync(product);
                _logger.LogInformation("Product added successfully with ID: {ProductId}", addedProduct.id);
                return Ok("Product added successfully.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error adding product with name: {ProductName}", productDto.Name);
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("allProductsinventory")] 
        //[Authorize(AuthenticationSchemes = "Roles", Policy = "RequireAdminRole")]
        //[Authorize(AuthenticationSchemes = "Roles", Policy = "RequireCashierRole")]
        [Authorize(AuthenticationSchemes = "Roles", Policy = "RequireAdminOrCashierRole")]
        public async Task<ActionResult> GetProductsAsync()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all products from inventory.");
                var products = await _inventoryManagerService.TrackProductInventory();
                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
                _logger.LogInformation("Successfully retrieved all products from inventory.");
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products from inventory.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(AuthenticationSchemes = "Roles", Policy = "RequireAdminRole")]
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(string id)
        {
            if (id==null)
            {
                _logger.LogWarning("Invalid product ID: {ProductId}", id);
                return BadRequest("Invalid product ID.");
            }

            try
            {
                var product = await _inventoryManagerService.FindProductByIDAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product not found with ID: {ProductId}", id);
                    return NotFound("Product not found.");
                }
                var productDto = _mapper.Map<ProductDto>(product);
                _logger.LogInformation("Product retrieved successfully with ID: {ProductId}", id);
                return Ok(productDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {ProductId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update/{id}")]
        [Authorize(AuthenticationSchemes = "Roles", Policy = "RequireAdminRole")]
        //Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult> UpdateProduct(string id, [FromBody] UpdateProductDto productDto)
        {
            if (id == null)
            {
                _logger.LogWarning("Invalid product ID: {ProductId}", id);
                return BadRequest("Invalid product ID.");
            }

            try
            {
                var product = _mapper.Map<Product>(productDto);
                var success = await _inventoryManagerService.UpdateProductAsync(id, product);
                if (success)
                {
                    _logger.LogInformation("Product updated successfully with ID: {ProductId}", id);
                    return Ok("Product updated successfully.");
                }

                _logger.LogWarning("Product not found with ID: {ProductId}", id);
                return NotFound("Product not found.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(AuthenticationSchemes = "Roles", Policy = "RequireAdminRole")]
        // [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            if (id == null)
            {
                _logger.LogWarning("Invalid product ID: {ProductId}", id);
                return BadRequest("Invalid product ID.");
            }

            try
            {
                var success = await _inventoryManagerService.RemoveProductAsync(id);
                if (success)
                {
                    _logger.LogInformation("Product deleted successfully with ID: {ProductId}", id);
                    return Ok("Product deleted successfully.");
                }

                _logger.LogWarning("Product not found with ID: {ProductId}", id);
                return NotFound("Product not found.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
                return BadRequest(ex.Message);
            }
        }
       // [Authorize(Policy = "RequireAdminRole")]

        [HttpPut("receive-stock/{id}")]
        [Authorize(AuthenticationSchemes = "Roles", Policy = "RequireAdminRole")]
        public async Task<ActionResult> ReceiveNewStock(string id, [FromQuery] int quantity)
        {
            if (id == null)
            {
                _logger.LogWarning("Invalid product ID: {ProductId}", id);
                return BadRequest("Invalid product ID.");
            }
            if (quantity <= 0)
            {
                _logger.LogWarning("Invalid quantity for stock receipt: {Quantity}", quantity);
                return BadRequest("Quantity must be greater than zero.");
            }

            try
            {
                var success = await _inventoryManagerService.ReceiveNewStockAsync(id, quantity);
                _logger.LogInformation("Stock received successfully for product ID: {ProductId} with quantity: {Quantity}", id, quantity);
                return Ok("Stock received successfully.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Product not found for stock receipt with ID: {ProductId}", id);
                return NotFound("Product not found.");
            }
        }

        [HttpPut("reduce-stock/{id}")]
        // [Authorize(Policy = "RequireAdminRole")]
        [Authorize(AuthenticationSchemes = "Roles", Policy = "RequireAdminRole")]
        public async Task<ActionResult> ReduceStock(string id, [FromQuery] int quantity)
        {
            if (id == null)
            {
                _logger.LogWarning("Invalid product ID: {ProductId}", id);
                return BadRequest("Invalid product ID.");
            }
            // Check if the quantity is valid
            if (quantity <= 0)
            {
                _logger.LogWarning("Invalid quantity for stock reduction: {Quantity}", quantity);
                return BadRequest("Quantity must be greater than zero.");
            }

            try
            {
                // Find the product by ID
                var product = await _inventoryManagerService.FindProductByIDAsync(id);

                // Check if the product exists
                if (product == null)
                {
                    _logger.LogWarning("Product not found for stock reduction with ID: {ProductId}", id);
                    return NotFound("Product not found.");
                }

                // Check if the quantity to reduce is more than the available stock
                if (product.Quantity < quantity)
                {
                    _logger.LogWarning("Insufficient stock for product ID: {ProductId}. Available: {AvailableQuantity}, Requested: {RequestedQuantity}", id, product.Quantity, quantity);
                    return BadRequest("Quantity exceeds available stock.");
                }

                // Proceed with stock reduction
                var success = await _inventoryManagerService.ReduceStockAsync(id, quantity);
                if (success == null)
                {
                    _logger.LogError("Failed to reduce stock for product ID: {ProductId}", id);
                    return StatusCode(500, "Failed to reduce stock. Please try again later.");
                }

                _logger.LogInformation("Stock reduced successfully for product ID: {ProductId} with quantity: {Quantity}", id, quantity);
                return Ok("Stock reduced successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reducing stock for product ID: {ProductId}", id);
                return StatusCode(500, "An unexpected error occurred. Please contact support.");
            }
        }

    }
}
