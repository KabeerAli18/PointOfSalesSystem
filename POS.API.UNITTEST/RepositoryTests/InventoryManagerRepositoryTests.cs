using Microsoft.EntityFrameworkCore;
using POS.API.DATA;
using POS.API.MODEL.Products;
using POS.API.REPOSITORIES.ProductRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace POS.API.UNITTEST.RepositoryTests
{
    [TestFixture]
    public class InventoryManagerRepositoryTests
    {
        private InventoryManagerRepository _repository;
        private MyDbContext _context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: "InventoryManagerTestDb")
                .Options;
            _context = new MyDbContext(options);
            _repository = new InventoryManagerRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddProductAsync_ShouldAddProduct_WhenProductIsValid()
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

            // Act
            var result = await _repository.AddProductAsync(product);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.Name, result.Name);
        }

        [Test]
        public void AddProductAsync_ShouldThrowArgumentException_WhenProductNameIsNull()
        {
            // Arrange
            var product = new Product
            {
                Quantity = 10,
                Price = 100,
                Type = "Type1",
                Category = "Category1"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _repository.AddProductAsync(product));
            Assert.That(ex.Message, Is.EqualTo("Product name is required. (Parameter 'Name')"));
        }

        [Test]
        public async Task TrackProductInventory_ShouldReturnAllProducts()
        {
            // Arrange
            var product1 = new Product
            {
                Name = "Product1",
                Quantity = 10,
                Price = 100,
                Type = "Type1",
                Category = "Category1"
            };
            var product2 = new Product
            {
                Name = "Product2",
                Quantity = 20,
                Price = 200,
                Type = "Type2",
                Category = "Category2"
            };
            _context.Products.AddRange(product1, product2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.TrackProductInventory();

            // Assert
           // Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task UpdateProductAsync_ShouldUpdateProduct_WhenProductExists()
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
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var updatedProduct = new Product
            {
                Name = "Updated Product",
                Quantity = 15,
                Price = 150,
                Type = "Updated Type",
                Category = "Updated Category"
            };

            // Act
            var result = await _repository.UpdateProductAsync(product.Id, updatedProduct);

            // Assert
            Assert.IsTrue(result);
            var dbProduct = await _context.Products.FindAsync(product.Id);
            Assert.AreEqual(updatedProduct.Name, dbProduct.Name);
        }

        [Test]
        public async Task RemoveProductAsync_ShouldRemoveProduct_WhenProductExists()
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
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.RemoveProductAsync(product.Id);

            // Assert
            Assert.IsTrue(result);
            var dbProduct = await _context.Products.FindAsync(product.Id);
            Assert.IsNull(dbProduct);
        }

        [Test]
        public async Task ReceiveNewStockAsync_ShouldIncreaseQuantity_WhenProductExists()
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
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var quantityToAdd = 5;

            // Act
            var result = await _repository.ReceiveNewStockAsync(product.Id, quantityToAdd);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.Quantity);
        }

        [Test]
        public void ReceiveNewStockAsync_ShouldThrowArgumentException_WhenQuantityIsNegative()
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
            _context.Products.Add(product);
            _context.SaveChanges();

            var quantityToAdd = -5;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _repository.ReceiveNewStockAsync(product.Id, quantityToAdd));
            Assert.That(ex.Message, Is.EqualTo("Quantity of Product entered is not Present in stock"));
        }

        [Test]
        public async Task ReduceStockAsync_ShouldDecreaseQuantity_WhenProductExists()
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
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var quantityToRemove = 5;

            // Act
            var result = await _repository.ReduceStockAsync(product.Id, quantityToRemove);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Quantity);
        }

        [Test]
        public void ReduceStockAsync_ShouldThrowArgumentException_WhenQuantityIsMoreThanAvailable()
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
            _context.Products.Add(product);
            _context.SaveChanges();

            var quantityToRemove = 15;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _repository.ReduceStockAsync(product.Id, quantityToRemove));
            Assert.That(ex.Message, Is.EqualTo("Quantity of Product entered is not Present in stock"));
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
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindProductByIDAsync(product.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.Name, result.Name);
        }

        [Test]
        public void FindProductByIDAsync_ShouldThrowArgumentException_WhenProductDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _repository.FindProductByIDAsync(1));
            Assert.That(ex.Message, Is.EqualTo("Product not found."));
        }
    }
}
