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

namespace WebApisPointOfSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> AddProduct([FromBody] CreateProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                _logger.LogInformation("Attempting to add a new product with name: {ProductName}", product.Name);
                var addedProduct = await _inventoryManagerService.AddProductAsync(product);
                _logger.LogInformation("Product added successfully with ID: {ProductId}", addedProduct.Id);
                return Ok("Product added successfully.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error adding product with name: {ProductName}", productDto.Name);
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("allProductsinventory")]
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

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            if (id <= 0)
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
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto productDto)
        {
            if (id <= 0)
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
        public async Task<ActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
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

        [HttpPut("receive-stock/{id}")]
        public async Task<ActionResult> ReceiveNewStock(int id, [FromQuery] int quantity)
        {
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
        public async Task<ActionResult> ReduceStock(int id, [FromQuery] int quantity)
        {
            if (quantity <= 0)
            {
                _logger.LogWarning("Invalid quantity for stock reduction: {Quantity}", quantity);
                return BadRequest("Quantity must be greater than zero.");
            }

            try
            {
                var success = await _inventoryManagerService.ReduceStockAsync(id, quantity);
                _logger.LogInformation("Stock reduced successfully for product ID: {ProductId} with quantity: {Quantity}", id, quantity);
                return Ok("Stock reduced successfully.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Product not found for stock reduction with ID: {ProductId}", id);
                return NotFound("Product not found.");
            }
        }
    }
}
