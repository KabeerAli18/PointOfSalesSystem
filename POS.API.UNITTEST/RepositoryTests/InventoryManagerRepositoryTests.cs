//using Microsoft.Azure.Cosmos;
//using NUnit.Framework;
//using POS.API.MODEL.Products;
//using POS.API.REPOSITORIES.ProductRepository;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace POS.API.UNITTEST.RepositoryTests
//{
//    [TestFixture]
//    public class InventoryManagerRepositoryTests
//    {
//        private InventoryManagerCosmosRepository _repository;
//        private CosmosClient _cosmosClient;
//        private Container _container;

//        [SetUp]
//        public async Task SetUp()
//        {
//           // _cosmosClient = new CosmosClient(";");
//            var database = await _cosmosClient.CreateDatabaseIfNotExistsAsync("POS");
//            _container = await database.Database.CreateContainerIfNotExistsAsync("Products", "/Category");

//            _repository = new InventoryManagerCosmosRepository(_cosmosClient, "POS", "Products");
//        }

//        [TearDown]
//        public async Task TearDown()
//        {
//            await _cosmosClient.GetDatabase("InventoryManagerTestDb").DeleteAsync();
//            _cosmosClient.Dispose();
//        }

//        [Test]
//        public async Task AddProductAsync_ShouldAddProduct_WhenProductIsValid()
//        {
//            // Arrange
//            var product = new Product
//            {
//                Name = "Test Product",
//                Quantity = 10,
//                Price = 100,
//                Type = "Type1",
//                Category = "Category1"
//            };

//            // Act
//            var result = await _repository.AddProductAsync(product);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(product.Name, result.Name);
//        }

//        [Test]
//        public void AddProductAsync_ShouldThrowArgumentException_WhenProductNameIsNull()
//        {
//            // Arrange
//            var product = new Product
//            {
//                Quantity = 10,
//                Price = 100,
//                Type = "Type1",
//                Category = "Category1"
//            };

//            // Act & Assert
//            var ex = Assert.ThrowsAsync<ArgumentException>(() => _repository.AddProductAsync(product));
//            Assert.That(ex.Message, Is.EqualTo("Product name is required. (Parameter 'Name')"));
//        }

//        //[Test]
//        //public async Task TrackProductInventory_ShouldReturnAllProducts()
//        //{
//        //    // Arrange
//        //    var product1 = new Product
//        //    {
//        //        Name = "Product1",
//        //        Quantity = 10,
//        //        Price = 100,
//        //        Type = "Type1",
//        //        Category = "Category1"
//        //    };
//        //    var product2 = new Product
//        //    {
//        //        Name = "Product2",
//        //        Quantity = 20,
//        //        Price = 200,
//        //        Type = "Type2",
//        //        Category = "Category2"
//        //    };
//        //    await _repository.AddProductAsync(product1);
//        //    await _repository.AddProductAsync(product2);

//        //    // Act
//        //    var result = await _repository.TrackProductInventory();

//        //    // Assert
//        //    Assert.AreEqual(2, result.Count);
//        //}

//        [Test]
//        public async Task UpdateProductAsync_ShouldUpdateProduct_WhenProductExists()
//        {
//            // Arrange
//            var product = new Product
//            {
//                Name = "Product1",
//                Quantity = 10,
//                Price = 100,
//                Type = "Type1",
//                Category = "Category1"
//            };
//            var createdProduct = await _repository.AddProductAsync(product);

//            var updatedProduct = new Product
//            {
//                Name = "Updated Product",
//                Quantity = 15,
//                Price = 150,
//                Type = "Updated Type",
//                Category = "Updated Category"
//            };

//            // Act
//            var result = await _repository.UpdateProductAsync(createdProduct.id, updatedProduct);

//            // Assert
//            Assert.IsTrue(result);
//            var dbProduct = await _repository.FindProductByIDAsync(createdProduct.id);
//            Assert.AreEqual(updatedProduct.Name, dbProduct.Name);
//        }

//        [Test]
//        public async Task RemoveProductAsync_ShouldRemoveProduct_WhenProductExists()
//        {
//            // Arrange
//            var product = new Product
//            {
//                Name = "Product1",
//                Quantity = 10,
//                Price = 100,
//                Type = "Type1",
//                Category = "Category1"
//            };
//            var createdProduct = await _repository.AddProductAsync(product);

//            // Act
//            var result = await _repository.RemoveProductAsync(createdProduct.id);

//            // Assert
//            Assert.IsTrue(result);
//            var dbProduct = await _repository.FindProductByIDAsync(createdProduct.id);
//            Assert.IsNull(dbProduct);
//        }

//        [Test]
//        public async Task ReceiveNewStockAsync_ShouldIncreaseQuantity_WhenProductExists()
//        {
//            // Arrange
//            var product = new Product
//            {
//                Name = "Product1",
//                Quantity = 10,
//                Price = 100,
//                Type = "Type1",
//                Category = "Category1"
//            };
//            var createdProduct = await _repository.AddProductAsync(product);

//            var quantityToAdd = 5;

//            // Act
//            var result = await _repository.ReceiveNewStockAsync(createdProduct.id, quantityToAdd);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(15, result.Quantity);
//        }

//        [Test]
//        public void ReceiveNewStockAsync_ShouldThrowArgumentException_WhenQuantityIsNegative()
//        {
//            // Arrange
//            var product = new Product
//            {
//                Name = "Product1",
//                Quantity = 10,
//                Price = 100,
//                Type = "Type1",
//                Category = "Category1"
//            };

//            var quantityToAdd = -5;

//            // Act & Assert
//            var ex = Assert.ThrowsAsync<ArgumentException>(() => _repository.ReceiveNewStockAsync(product.id, quantityToAdd));
//            Assert.That(ex.Message, Is.EqualTo("Quantity of Product entered is not Present in stock"));
//        }

//        [Test]
//        public async Task ReduceStockAsync_ShouldDecreaseQuantity_WhenProductExists()
//        {
//            // Arrange
//            var product = new Product
//            {
//                Name = "Product1",
//                Quantity = 10,
//                Price = 100,
//                Type = "Type1",
//                Category = "Category1"
//            };
//            var createdProduct = await _repository.AddProductAsync(product);

//            var quantityToRemove = 5;

//            // Act
//            var result = await _repository.ReduceStockAsync(createdProduct.id, quantityToRemove);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(5, result.Quantity);
//        }

//        //[Test]
//        //public void ReduceStockAsync_ShouldThrowArgumentException_WhenQuantityIsMoreThanAvailable()
//        //{
//        //    // Arrange
//        //    var product = new Product
//        //    {
//        //        Name = "Product1",
//        //        Quantity = 10,
//        //        Price = 100,
//        //        Type = "Type1",
//        //        Category = "Category1"
//        //    };
//        //    var createdProduct = await _repository.AddProductAsync(product);

//        //    var quantityToRemove = 15;

//        //    // Act & Assert
//        //    var ex = Assert.ThrowsAsync<ArgumentException>(() => _repository.ReduceStockAsync(createdProduct.id, quantityToRemove));
//        //    Assert.That(ex.Message, Is.EqualTo("Quantity of Product entered is not Present in stock"));
//        //}

//        [Test]
//        public async Task FindProductByIDAsync_ShouldReturnProduct_WhenProductExists()
//        {
//            // Arrange
//            var product = new Product
//            {
//                Name = "Product1",
//                Quantity = 10,
//                Price = 100,
//                Type = "Type1",
//                Category = "Category1"
//            };
//            var createdProduct = await _repository.AddProductAsync(product);

//            // Act
//            var result = await _repository.FindProductByIDAsync(createdProduct.id);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(createdProduct.Name, result.Name);
//        }

//        [Test]
//        public void FindProductByIDAsync_ShouldThrowArgumentException_WhenProductDoesNotExist()
//        {
//            // Act & Assert
//            var ex = Assert.ThrowsAsync<ArgumentException>(() => _repository.FindProductByIDAsync("abc"));
//            Assert.That(ex.Message, Is.EqualTo("Product not found."));
//        }
//    }
//}
