using Microsoft.Azure.Cosmos;
using POS.API.MODEL.Products;
using POS.API.MODEL.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS.API.REPOSITORIES.SalesTransactionRepository
{
    public class SalesTransactionCosmosRepository : ISalesTransactionRepository
    {
        private readonly Container _container;
        private readonly Container _productContainer;

        public SalesTransactionCosmosRepository(CosmosClient dbClient, string databaseName, string salesContainerName, string productContainerName)
        {
            _container = dbClient.GetContainer(databaseName, salesContainerName);
            _productContainer = dbClient.GetContainer(databaseName, productContainerName);
        }

        public async Task AddProductToSaleAsync(string productId, int quantity)
        {
            try
            {
                // Log the productId and quantity for debugging
                Console.WriteLine($"Attempting to add product to sale. ProductId: {productId}, Quantity: {quantity}");

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

                if (product.Quantity < quantity)
                {
                    Console.WriteLine("Insufficient quantity in stock.");
                    throw new ArgumentException("Insufficient quantity in stock.");
                }

                product.Quantity -= quantity;

                // Update product quantity in inventory
                await _productContainer.ReplaceItemAsync(product, product.id, new PartitionKey(product.Category));

                var saleItem = new SaleItem
                {
                    id = Guid.NewGuid().ToString(),
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price,
                    SalesItemName = product.Name
                };

                // Add sale item to sales container
                await _container.CreateItemAsync(saleItem, new PartitionKey(productId));
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

        public async Task<decimal> CalculateTotalSalesAmountAsync()
        {
            try
            {
                var query = "SELECT * FROM c";
                var iterator = _container.GetItemQueryIterator<SaleItem>(new QueryDefinition(query));
                var results = new List<SaleItem>();
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    results.AddRange(response);
                }
                // Clear all sale items after generating the receipt
               // await ClearSaleItemsAsync();
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

        public async Task<SalesReceiptResponse> GenerateSalesTransactionsReceiptAsync()
        {
            try
            {
                
                var query = "SELECT * FROM c";
                var iterator = _container.GetItemQueryIterator<SaleItem>(new QueryDefinition(query));
                var saleItems = new List<SaleItem>();
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    saleItems.AddRange(response);
                }

                var receiptItems = saleItems.Select(item => new SaleItemResponse
                {
                    ProductName = item.SalesItemName,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList();

                var receiptResponse = new SalesReceiptResponse
                {
                    ReceiptHeader = "Sales Transaction Receipt/Invoice",
                    SaleItems = receiptItems,
                    TotalAmount = await CalculateTotalSalesAmountAsync()
                };

                // Clear all sale items after generating the receipt
                await ClearSaleItemsAsync();

                return receiptResponse;
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

        public async Task ClearSaleItemsAsync()
        {
            try
            {
                var query = "SELECT * FROM c";
                var iterator = _container.GetItemQueryIterator<SaleItem>(new QueryDefinition(query));
                var saleItems = new List<SaleItem>();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    saleItems.AddRange(response);
                }

                foreach (var saleItem in saleItems)
                {
                    await _container.DeleteItemAsync<SaleItem>(saleItem.id, new PartitionKey(saleItem.ProductId));
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







