using Microsoft.Azure.Cosmos;
using POS.API.MODEL.Purchase;
using POS.API.MODEL.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS.API.REPOSITORIES.PurchaseTransactionRepository
{
    public class PurchaseTransactionCosmosRepository : IPurchaseTransactionRepository
    {
        private readonly Container _container;
        private readonly Container _productContainer;

        public PurchaseTransactionCosmosRepository(CosmosClient dbClient, string databaseName, string purchaseContainerName, string productContainerName)
        {
            _container = dbClient.GetContainer(databaseName, purchaseContainerName);
            _productContainer = dbClient.GetContainer(databaseName, productContainerName);
        }

        public async Task AddProductToPurchaseOrderAsync(string productId, int quantity)
        {
            try
            {
                // Log the productId and quantity for debugging
                Console.WriteLine($"Attempting to add product to purchase order. ProductId: {productId}, Quantity: {quantity}");

                // Read the product using the correct partition key (Category)
                var productQuery = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                    .WithParameter("@id", productId);

                var productIterator = _productContainer.GetItemQueryIterator<Product>(productQuery);
                Product product = new Product();

                while (productIterator.HasMoreResults)
                {
                    var response = await productIterator.ReadNextAsync();
                    product = response.FirstOrDefault();
                }

                if (product == null)
                {
                    Console.WriteLine("Product not found in inventory.");
                    throw new ArgumentException("Product not found in inventory.");
                }

                product.Quantity += quantity; // Increase the product quantity

                // Update product quantity in inventory
                await _productContainer.ReplaceItemAsync(product, product.id, new PartitionKey(product.Category));

                var purchaseItem = new PurchaseItem
                {
                    id = Guid.NewGuid().ToString(),
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price,
                    PurchaseItemName = product.Name
                };

                // Add purchase item to purchase container
                await _container.CreateItemAsync(purchaseItem, new PartitionKey(productId));
            }
            catch (CosmosException ex)
            {
                // Handle Cosmos DB related exceptions
                Console.WriteLine($"Cosmos DB error: {ex.Message}");
                throw new ArgumentException($"Cosmos DB error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw new ArgumentException($"An error occurred: {ex.Message}", ex);
            }
        }

        public async Task<decimal> CalculateTotalPurchaseAmountAsync()
        {
            try
            {
                
                var query = "SELECT * FROM c";
                var iterator = _container.GetItemQueryIterator<PurchaseItem>(new QueryDefinition(query));
                var results = new List<PurchaseItem>();
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    results.AddRange(response);
                }
                //to Clear the Last History
                //await ClearPurchaseItemsAsync();
                return results.Sum(item => item.Price * item.Quantity);
            }
            catch (CosmosException ex)
            {
                // Handle Cosmos DB related exceptions
                Console.WriteLine($"Cosmos DB error: {ex.Message}");
                throw new ArgumentException($"Cosmos DB error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw new ArgumentException($"An error occurred: {ex.Message}", ex);
            }
        }

        public async Task<PurchaseReceiptResponse> GeneratePurchaseReceiptInvoiceAsync()
        {
            try
            {
                var query = "SELECT * FROM c";
                var iterator = _container.GetItemQueryIterator<PurchaseItem>(new QueryDefinition(query));
                var purchaseItems = new List<PurchaseItem>();
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    purchaseItems.AddRange(response);
                }

                var receiptItems = purchaseItems.Select(item => new PurchaseItemResponse
                {
                    ProductName = item.PurchaseItemName,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList();


                var res= new PurchaseReceiptResponse
                {
                    ReceiptHeader = "Purchase Receipt/Invoice",
                    PurchaseItems = receiptItems,
                    TotalAmount = await CalculateTotalPurchaseAmountAsync()
                };

                await ClearPurchaseItemsAsync();
                return res;
            }
            catch (CosmosException ex)
            {
                // Handle Cosmos DB related exceptions
                Console.WriteLine($"Cosmos DB error: {ex.Message}");
                throw new ArgumentException($"Cosmos DB error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw new ArgumentException($"An error occurred: {ex.Message}", ex);
            }
        }

        public async Task ClearPurchaseItemsAsync()
        {
            try
            {
                var query = "SELECT * FROM c";
                var iterator = _container.GetItemQueryIterator<PurchaseItem>(new QueryDefinition(query));
                var purchaseItems = new List<PurchaseItem>();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    purchaseItems.AddRange(response);
                }

                foreach (var purchaseItem in purchaseItems)
                {
                    await _container.DeleteItemAsync<PurchaseItem>(purchaseItem.id, new PartitionKey(purchaseItem.ProductId));
                }
            }
            catch (CosmosException ex)
            {
                // Handle Cosmos DB related exceptions
                Console.WriteLine($"Cosmos DB error: {ex.Message}");
                throw new ArgumentException($"Cosmos DB error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw new ArgumentException($"An error occurred: {ex.Message}", ex);
            }
        }
    }
}
