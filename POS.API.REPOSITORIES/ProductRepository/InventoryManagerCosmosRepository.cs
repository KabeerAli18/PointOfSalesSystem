using Microsoft.Azure.Cosmos;
using POS.API.MODEL.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS.API.REPOSITORIES.ProductRepository
{
    public class InventoryManagerCosmosRepository : IInventoryManagerRepository
    {
        private readonly Container _container;

        public InventoryManagerCosmosRepository(CosmosClient dbClient, string databaseName, string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        // Add a new product
        public async Task<Product> AddProductAsync(Product product)
        {
            try
            {
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product), "Product is null.");
                }

                if (string.IsNullOrWhiteSpace(product.Name))
                {
                    throw new ArgumentException("Product name is required.", nameof(product.Name));
                }

                if (product.Quantity < 0)
                {
                    throw new ArgumentException("Product quantity cannot be negative.", nameof(product.Quantity));
                }

                if (product.Price < 0)
                {
                    throw new ArgumentException("Product price cannot be negative.", nameof(product.Price));
                }

                if (string.IsNullOrWhiteSpace(product.Type))
                {
                    throw new ArgumentException("Product type is required.", nameof(product.Type));
                }
                if (string.IsNullOrWhiteSpace(product.Category))
                {
                    throw new ArgumentException("Product category is required.", nameof(product.Category));
                }

                // Check if a product with the same name and type already exists
                var query = new QueryDefinition("SELECT * FROM c WHERE c.Name = @Name AND c.Type = @Type")
                    .WithParameter("@Name", product.Name)
                    .WithParameter("@Type", product.Type);

                var iterator = _container.GetItemQueryIterator<Product>(query);
                if (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    if (response.Any())
                    {
                        throw new ArgumentException("A product with the same name and type already exists.");
                    }
                }

                product.id = Guid.NewGuid().ToString();

                var response1 = await _container.CreateItemAsync(product, new PartitionKey(product.Category));
                return response1.Resource;
            }
            catch (CosmosException ex)
            {
                // Log the specific details of the CosmosException
                Console.WriteLine($"CosmosDB error: {ex.StatusCode} - {ex.Message}");
                throw new ArgumentException("CosmosDB error occurred. Please try again later.", ex);
            }
            catch(ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new ArgumentException("An unexpected error occurred. Please try again later.", ex);
            }
        }

        // View all products
        public async Task<IEnumerable<Product>> TrackProductInventory()
        {
            try
            {
                var query = "SELECT * FROM c";
                var iterator = _container.GetItemQueryIterator<Product>(new QueryDefinition(query));
                var results = new List<Product>();
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    results.AddRange(response);
                }
                return results;
            }
            catch (CosmosException ex)
            {
                // Log the specific details of the CosmosException
                Console.WriteLine($"CosmosDB error: {ex.StatusCode} - {ex.Message}");
                throw new ArgumentException("CosmosDB error occurred. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new ArgumentException("An unexpected error occurred. Please try again later.", ex);
            }
        }

        /*ReplaceItemAsync vs. UpsertItemAsync: 
        ReplaceItemAsync will replace the item with the specified ID within the same partition key, while 
        UpsertItemAsync may create a new item if the partition key is different.*/
        // Update a product
        public async Task<bool> UpdateProductAsync(string id, Product productUpdate)
        {
            try
            {
                var product = await FindProductByIDAsync(id);
                if (product != null)
                {
                    if (!string.IsNullOrEmpty(productUpdate.Name)) product.Name = productUpdate.Name;
                    product.Price = productUpdate.Price;
                    product.Quantity = productUpdate.Quantity;
                    if (!string.IsNullOrEmpty(productUpdate.Type)) product.Type = productUpdate.Type;

                    // Ensure the partition key remains the same during the update
                    if (!string.IsNullOrEmpty(productUpdate.Category) && productUpdate.Category != product.Category)
                    {
                        throw new ArgumentException("Product category cannot be changed during an update.");
                    }

                    var response = await _container.ReplaceItemAsync(product, id, new PartitionKey(product.Category));
                    return response.StatusCode == System.Net.HttpStatusCode.OK;
                }
                return false;
            }
            catch (CosmosException ex)
            {
                // Log the specific details of the CosmosException
                Console.WriteLine($"CosmosDB error: {ex.StatusCode} - {ex.Message}");
                throw new ArgumentException("CosmosDB error occurred. Please try again later.", ex);
            }
            catch (ArgumentException ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new ArgumentException("An unexpected error occurred. Please try again later.", ex);
            }
        }

        // Remove a product
        public async Task<bool> RemoveProductAsync(string id)
        {
            try
            {
                var product = await FindProductByIDAsync(id);
                if (product != null)
                {
                    var response = await _container.DeleteItemAsync<Product>(id, new PartitionKey(product.Category));
                    return response.StatusCode == System.Net.HttpStatusCode.NoContent;
                }
                else
                {
                    throw new ArgumentException("Id to Delete not Found", id);
                }
         
            }
            catch (CosmosException ex)
            {
                // Log the specific details of the CosmosException
                Console.WriteLine($"CosmosDB error: {ex.StatusCode} - {ex.Message}");
                throw new ArgumentException("CosmosDB error occurred. Please try again later.", ex);
            }
            catch (ArgumentException ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new ArgumentException("An unexpected error occurred. Please try again later.", ex);
            }
        }

        public async Task<Product> ReceiveNewStockAsync(string id, int quantity)
        {
            try
            {
                var product = await FindProductByIDAsync(id);

                if (product != null && quantity > 0)
                {
                    product.Quantity += quantity;
                    await UpdateProductAsync(id, product);
                    return product;
                }
                else
                {
                    throw new ArgumentException("Invalid quantity for new stock.");
                }
            }
            catch (ArgumentException ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error: {ex.Message}");
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new ArgumentException("An unexpected error occurred. Please try again later.", ex);
            }
        }

        public async Task<Product> ReduceStockAsync(string id, int quantity)
        {
            try
            {
                var product = await FindProductByIDAsync(id);

                if (product != null && quantity > 0 && quantity <= product.Quantity)
                {
                    product.Quantity -= quantity;
                    await UpdateProductAsync(id, product);
                    return product;
                }
                else
                {
                    throw new ArgumentException("Invalid quantity for stock reduction.");
                }
            }
            catch (ArgumentException ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error: {ex.Message}");
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new ArgumentException("An unexpected error occurred. Please try again later.", ex);
            }
        }

        // Find a product by ID
        public async Task<Product> FindProductByIDAsync(string id)
        {
            try
            {
                var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id").WithParameter("@id", id);

                var iterator = _container.GetItemQueryIterator<Product>(query);
                var results = new List<Product>();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    results.AddRange(response);
                }

                var product = results.FirstOrDefault();
                if (product == null)
                {
                    throw new ArgumentException($"Product with id {id} not found.");
                }
                return product;
            }
            catch (CosmosException ex)
            {
                // Log the specific details of the CosmosException
                Console.WriteLine($"CosmosDB error: {ex.StatusCode} - {ex.Message}");
                throw new ArgumentException("CosmosDB error occurred here. Please try again later.", ex);
            }
            catch(ArgumentException ex)
            {
                throw new ArgumentException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new ArgumentException("An unexpected error occurred. Please try again later.", ex);
            }
        }

        
    }
}
