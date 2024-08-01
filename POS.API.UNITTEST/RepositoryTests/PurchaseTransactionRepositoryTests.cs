

using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using POS.API.DATA;
using POS.API.MODEL.Products;
using POS.API.MODEL.Purchase;
using POS.API.REPOSITORIES.PurchaseTransactionRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS.API.UNITTEST.RepositoryTests
{
    [TestFixture]
    public class PurchaseTransactionRepositoryTests
    {
        private PurchaseTransactionRepository _repository;
        private MyDbContext _context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: "PurchaseTransactionTestDb")
                .Options;
            _context = new MyDbContext(options);
            _repository = new PurchaseTransactionRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddProductToPurchaseOrderAsync_ShouldAddProduct_WhenProductExistsAndQuantityIsSufficient()
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
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            await _repository.AddProductToPurchaseOrderAsync(product.id, 5);

            // Assert
            var purchaseItem = _context.PurchaseItems.FirstOrDefault();
            Assert.IsNotNull(purchaseItem);
            Assert.AreEqual(product.id, purchaseItem.ProductId);
            Assert.AreEqual(5, purchaseItem.Quantity);
        }

        [Test]
        public void AddProductToPurchaseOrderAsync_ShouldThrowArgumentException_WhenProductDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _repository.AddProductToPurchaseOrderAsync("abc", 5));
            Assert.That(ex.Message, Is.EqualTo("Product not found in inventory."));
        }

        [Test]
        public void AddProductToPurchaseOrderAsync_ShouldThrowArgumentException_WhenQuantityIsInsufficient()
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
            _context.Products.Add(product);
            _context.SaveChanges();

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _repository.AddProductToPurchaseOrderAsync(product.id, 15));
            Assert.That(ex.Message, Is.EqualTo("Insufficient quantity in stock."));
        }

        //[Test]
        //public async Task CalculateTotalPurchaseAmountAsync_ShouldReturnTotalAmountOfAllPurchaseItems()
        //{
        //    // Arrange
        //    var product = new Product
        //    {
        //        Name = "Test Product",
        //        Quantity = 10,
        //        Price = 100,
        //        Type = "Type1",
        //        Category = "Category1"
        //    };
        //    _context.Products.Add(product);
        //    await _context.SaveChangesAsync();

        //    var purchaseItem = new PurchaseItem
        //    {
        //        ProductId = product.Id,
        //        Quantity = 2,
        //        Price = product.Price,
        //        PurchaseItemName = product.Name
        //    };
        //    _context.PurchaseItems.Add(purchaseItem);
        //    await _context.SaveChangesAsync();

        //    // Act
        //    var result = await _repository.CalculateTotalPurchaseAmountAsync();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(200, result);
        //}

        //[Test]
        //public async Task GeneratePurchaseReceiptInvoiceAsync_ShouldReturnPurchaseReceipt()
        //{
        //    // Arrange
        //    var product = new Product
        //    {
        //        Name = "Test Product",
        //        Quantity = 10,
        //        Price = 100,
        //        Type = "Type1",
        //        Category = "Category1"
        //    };
        //    _context.Products.Add(product);
        //    await _context.SaveChangesAsync();

        //    var purchaseItem = new PurchaseItem
        //    {
        //        ProductId = product.Id,
        //        Quantity = 2,
        //        Price = product.Price,
        //        PurchaseItemName = product.Name
        //    };
        //    _context.PurchaseItems.Add(purchaseItem);
        //    await _context.SaveChangesAsync();

        //    // Act
        //    var result = await _repository.GeneratePurchaseReceiptInvoiceAsync();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("Purchase Receipt/Invoice", result.ReceiptHeader);
        //    Assert.AreEqual(1, result.PurchaseItems.Count);
        //    Assert.AreEqual(200, result.TotalAmount);
        //}


        [Test]
        public async Task ClearPurchaseItemsAsync_ShouldRemoveAllPurchaseItems()
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
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var purchaseItem = new PurchaseItem
            {
                ProductId = product.id,
                Quantity = 2,
                Price = product.Price,
                PurchaseItemName = product.Name
            };
            _context.PurchaseItems.Add(purchaseItem);
            await _context.SaveChangesAsync();

            // Act
            await _repository.ClearPurchaseItemsAsync();

            // Assert
            var purchaseItems = await _context.PurchaseItems.ToListAsync();
            Assert.AreEqual(0, purchaseItems.Count);
        }


        //Debugger Test


    }
}
