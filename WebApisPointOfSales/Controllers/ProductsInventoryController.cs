using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PointOfSales;
using PointOfSales.Entities;
using PointOfSales.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WebApisPointOfSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsInventoryController : ControllerBase
    {
        private readonly ILogger<ProductsInventoryController> _logger;
        private readonly MyDbContext _myDbContext;

        public ProductsInventoryController(MyDbContext myDbContext, ILogger<ProductsInventoryController> logger)
        {
            _myDbContext = myDbContext;
            _logger = logger;
            InventoryManager.Initialize(myDbContext);
        }

        // Add a New Product to Inventory
        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            try
            {
                _logger.LogInformation("Attempting to add a new product with ID: {ProductId}", product.Id);
                var prod = await InventoryManager.AddProductAsync(product);
                _logger.LogInformation("Product added successfully with ID: {ProductId}", product.Id);
                return Ok("Product added successfully.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error adding product with ID: {ProductId}", product.Id);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("allProductsinventory")]
        // Read all the products from inventory
        public async Task<ActionResult> GetProductsAsync()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all products from inventory.");
                var productinventorylist = await InventoryManager.TrackProductInventory();
                _logger.LogInformation("Successfully retrieved all products from inventory.");
                return Ok(productinventorylist);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error retrieving products from inventory.");
                return BadRequest(ex.Message);
            }
        }

        // Get Product By ID
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            if (id <= 0) // Check if the id is a valid positive number
            {
                _logger.LogWarning("Invalid product ID: {ProductId}", id);
                return BadRequest("Invalid product ID.");
            }

            var product = await InventoryManager.FindProductByIDAsync(id);
            if (product != null)
            {
                _logger.LogInformation("Product retrieved successfully with ID: {ProductId}", id);
                return Ok(product);
            }

            _logger.LogWarning("Product not found with ID: {ProductId}", id);
            return NotFound("Product not found.");
        }

        [HttpPut("Update/{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, [FromBody] Product product)
        {
            if (product == null)
            {
                _logger.LogWarning("Invalid product data for ID: {ProductId}", id);
                return BadRequest("Invalid product data.");
            }

            var success = await InventoryManager.UpdateProductAsync(id, product);
            if (success)
            {
                _logger.LogInformation("Product updated successfully with ID: {ProductId}", id);
                return Ok("Product updated successfully.");
            }

            _logger.LogWarning("Product not found with ID: {ProductId}", id);
            return NotFound("Product not found.");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            if (id <= 0) // Check if the id is a valid positive number
            {
                _logger.LogWarning("Invalid product ID: {ProductId}", id);
                return BadRequest("Invalid product ID.");
            }

            var success = await InventoryManager.RemoveProductAsync(id);
            if (success)
            {
                _logger.LogInformation("Product deleted successfully with ID: {ProductId}", id);
                return Ok("Product deleted successfully.");
            }

            _logger.LogWarning("Product not found with ID: {ProductId}", id);
            return NotFound("Product not found.");
        }

        // Receive New Stock for an Item
        [HttpPut("receive-stock/{id}")]
        public async Task<ActionResult> ReceiveNewStock(int id, [FromQuery] int quantity)
        {
            if (quantity <= 0)
            {
                _logger.LogWarning("Invalid quantity for stock receipt: {Quantity}", quantity);
                return BadRequest("Quantity must be greater than zero.");
            }

            var success = await InventoryManager.ReceiveNewStockAsync(id, quantity);
            if (success != null)
            {
                _logger.LogInformation("Stock received successfully for product ID: {ProductId} with quantity: {Quantity}", id, quantity);
                return Ok("Stock received successfully.");
            }

            _logger.LogWarning("Product not found for stock receipt with ID: {ProductId}", id);
            return NotFound("Product not found.");
        }

        // Reduce the Stock
        [HttpPut("reduce-stock/{id}")]
        public async Task<ActionResult> ReduceStock(int id, [FromQuery] int quantity)
        {
            if (quantity <= 0)
            {
                _logger.LogWarning("Invalid quantity for stock reduction: {Quantity}", quantity);
                return BadRequest("Quantity must be greater than zero.");
            }

            var successProduct = await InventoryManager.ReduceStockAsync(id, quantity);
            if (successProduct != null)
            {
                _logger.LogInformation("Stock reduced successfully for product ID: {ProductId} with quantity: {Quantity}", id, quantity);
                return Ok("Stock reduced successfully.");
            }

            _logger.LogWarning("Product not found for stock reduction with ID: {ProductId}", id);
            return NotFound("Product not found.");
        }
    }
}
