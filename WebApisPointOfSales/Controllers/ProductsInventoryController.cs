using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PointOfSales;
using PointOfSales.Entities;
using PointOfSales.Services;
namespace WebApisPointOfSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsInventoryController : ControllerBase
    {
        public ProductsInventoryController(MyDbContext myDbContext)
        {
            InventoryManager.Initialize(myDbContext);
        }


        //Add a New Product to Inventory
        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            try
            {
                var prod=await InventoryManager.AddProductAsync(product);
                return Ok("Product Added successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("allProductsinventory")]
        //Read all the products from inventory
        public async Task<ActionResult> GetProductsAsync()
        {
            try {
                var productinventorylist = await InventoryManager.TrackProductInventory();
                return Ok(productinventorylist);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //gET PRODCUT BY ID
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            if (id <= 0) // Check if the id is a valid positive number
            {
                return BadRequest("Invalid product ID.");
            }

            var product = await InventoryManager.FindProductByIDAsync(id);
            if (product != null)
            {
                return Ok(product);
            }
            return NotFound("Product not found.");
        }

        [HttpPut("Update/{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, [FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Invalid product data.");
            }

            var success = await InventoryManager.UpdateProductAsync(id, product);
            if (success)
            {
                return Ok("Product updated successfully.");
            }
            return NotFound("Product not found.");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            if (id <= 0) // Check if the id is a valid positive number
            {
                return BadRequest("Invalid product ID.");
            }

            var success = await InventoryManager.RemoveProductAsync(id);
            if (success)
            {
                return Ok("Product deleted successfully.");
            }
            return NotFound("Product not found.");
        }


        // Receive New Stock for an Item
        [HttpPut("receive-stock/{id}")]
        public async Task<ActionResult> ReceiveNewStock(int id, [FromQuery] int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            var success = await InventoryManager.ReceiveNewStockAsync(id, quantity);
            if (success)
            {
                return Ok("Stock received successfully.");
            }
            return NotFound("Product not found.");
        }


        //Reduce the Stock

        [HttpPut("reduce-stock/{id}")]
        public async Task<ActionResult> ReduceStock(int id, [FromQuery] int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            var success = await InventoryManager.ReduceStockAsync(id, quantity);
            if (success)
            {
                return Ok("Stock reduced successfully.");
            }
            return NotFound("Product not found.");
        }




    }
}
