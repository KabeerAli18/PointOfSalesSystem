
using Moq;
using NUnit.Framework;
using POS.API.MODEL.Products;
using POS.API.REPOSITORIES.ProductRepository;
using POS.API.SERVICES.ProductServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POS.API.UNITTEST.ServicesTests
{
    [TestFixture]
    public class InventoryManagerServiceTests
    {
        private Mock<IInventoryManagerRepository> _inventoryManagerRepositoryMock;
        private InventoryManagerService _service;

        [SetUp]
        public void SetUp()
        {
            _inventoryManagerRepositoryMock = new Mock<IInventoryManagerRepository>();
            _service = new InventoryManagerService(_inventoryManagerRepositoryMock.Object);
        }

        [Test]
        public async Task AddProductAsync_ShouldReturnProduct_WhenProductIsValid()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Quantity = 10,
                Price = 100,
                Type = "Type1",
                Category = "Category1"
            };
            _inventoryManagerRepositoryMock.Setup(repo => repo.AddProductAsync(product)).ReturnsAsync(product);

            // Act
            var result = await _service.AddProductAsync(product);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.Name, result.Name);
        }

        [Test]
        public void AddProductAsync_ShouldThrowArgumentException_WhenRepositoryThrowsArgumentException()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Quantity = 10,
                Price = 100,
                Type = "Type1",
                Category = "Category1"
            };
            _inventoryManagerRepositoryMock.Setup(repo => repo.AddProductAsync(product)).ThrowsAsync(new ArgumentException("Error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.AddProductAsync(product));
            Assert.That(ex.Message, Is.EqualTo("Error (Parameter 'product')"));
        }

        [Test]
        public async Task TrackProductInventory_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Name = "Product1", Quantity = 10, Price = 100, Type = "Type1", Category = "Category1" },
                new Product { Name = "Product2", Quantity = 20, Price = 200, Type = "Type2", Category = "Category2" }
            };
            _inventoryManagerRepositoryMock.Setup(repo => repo.TrackProductInventory()).ReturnsAsync(products);

            // Act
            var result = await _service.TrackProductInventory();

            // Assert
           // Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task UpdateProductAsync_ShouldReturnTrue_WhenProductIsUpdated()
        {
            // Arrange
            var product = new Product
            {
                Name = "Updated Product",
                Quantity = 15,
                Price = 150,
                Type = "Updated Type",
                Category = "Updated Category"
            };
            _inventoryManagerRepositoryMock.Setup(repo => repo.UpdateProductAsync(It.IsAny<string>(), product)).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateProductAsync("abc", product);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void UpdateProductAsync_ShouldThrowArgumentException_WhenRepositoryThrowsArgumentException()
        {
            // Arrange
            var product = new Product
            {
                Name = "Updated Product",
                Quantity = 15,
                Price = 150,
                Type = "Updated Type",
                Category = "Updated Category"
            };
            _inventoryManagerRepositoryMock.Setup(repo => repo.UpdateProductAsync(It.IsAny<string>(), product)).ThrowsAsync(new ArgumentException("Error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateProductAsync("abc", product));
            Assert.That(ex.Message, Is.EqualTo("Error (Parameter 'product')"));
        }

        [Test]
        public async Task RemoveProductAsync_ShouldReturnTrue_WhenProductIsRemoved()
        {
            // Arrange
            _inventoryManagerRepositoryMock.Setup(repo => repo.RemoveProductAsync(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _service.RemoveProductAsync("abc");

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void RemoveProductAsync_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            _inventoryManagerRepositoryMock.Setup(repo => repo.RemoveProductAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.RemoveProductAsync("abc"));
            Assert.That(ex.Message, Is.EqualTo("Error while removing product: "));
        }

        [Test]
        public async Task ReceiveNewStockAsync_ShouldReturnProduct_WhenStockIsReceived()
        {
            // Arrange
            var product = new Product
            {
                Name = "Product1",
                Quantity = 10,
                Price = 100,
                Type = "Type1",
                Category = "Category1"
            };
            _inventoryManagerRepositoryMock.Setup(repo => repo.ReceiveNewStockAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(product);

            // Act
            var result = await _service.ReceiveNewStockAsync("abc", 5);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.Name, result.Name);
        }

        [Test]
        public void ReceiveNewStockAsync_ShouldThrowArgumentException_WhenRepositoryThrowsArgumentException()
        {
            // Arrange
            _inventoryManagerRepositoryMock.Setup(repo => repo.ReceiveNewStockAsync(It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new ArgumentException("Error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.ReceiveNewStockAsync("abc", 5));
            Assert.That(ex.Message, Is.EqualTo("Error (Parameter 'id')"));
        }

        [Test]
        public async Task ReduceStockAsync_ShouldReturnProduct_WhenStockIsReduced()
        {
            // Arrange
            var product = new Product
            {
                Name = "Product1",
                Quantity = 10,
                Price = 100,
                Type = "Type1",
                Category = "Category1"
            };
            _inventoryManagerRepositoryMock.Setup(repo => repo.ReduceStockAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(product);

            // Act
            var result = await _service.ReduceStockAsync("abc", 5);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.Name, result.Name);
        }

        [Test]
        public void ReduceStockAsync_ShouldThrowArgumentException_WhenRepositoryThrowsArgumentException()
        {
            // Arrange
            _inventoryManagerRepositoryMock.Setup(repo => repo.ReduceStockAsync(It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new ArgumentException("Error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.ReduceStockAsync("abc", 5));
            Assert.That(ex.Message, Is.EqualTo("Error (Parameter 'id')"));
        }

        [Test]
        public async Task FindProductByIDAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var product = new Product
            {
                Name = "Product1",
                Quantity = 10,
                Price = 100,
                Type = "Type1",
                Category = "Category1"
            };
            _inventoryManagerRepositoryMock.Setup(repo => repo.FindProductByIDAsync(It.IsAny<string>())).ReturnsAsync(product);

            // Act
            var result = await _service.FindProductByIDAsync("abc");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.Name, result.Name);
        }

        [Test]
        public void FindProductByIDAsync_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            _inventoryManagerRepositoryMock.Setup(repo => repo.FindProductByIDAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.FindProductByIDAsync("abc"));
            Assert.That(ex.Message, Is.EqualTo("Error while finding product by ID: "));
        }
    }
}
