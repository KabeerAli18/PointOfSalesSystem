using Moq;
using NUnit.Framework;
using POS.API.MODEL.Purchase;
using POS.API.REPOSITORIES.PurchaseTransactionRepository;
using POS.API.SERVICES.PurchaseServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POS.API.UNITTEST.ServicesTests
{
    [TestFixture]
    public class PurchaseTransactionServicesTests
    {
        private Mock<IPurchaseTransactionRepository> _mockRepository;
        private PurchaseTransactionServices _service;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<IPurchaseTransactionRepository>();
            _service = new PurchaseTransactionServices(_mockRepository.Object);
        }

        [Test]
        public async Task CalculateTotalPurchaseAmountAsync_ShouldReturnTotalAmount()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.CalculateTotalPurchaseAmountAsync())
                .ReturnsAsync(200);

            // Act
            var result = await _service.CalculateTotalPurchaseAmountAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result);
            _mockRepository.Verify(repo => repo.CalculateTotalPurchaseAmountAsync(), Times.Once);
        }

        [Test]
        public async Task GeneratePurchaseReceiptInvoiceAsync_ShouldReturnReceipt()
        {
            // Arrange
            var receipt = new PurchaseReceiptResponse
            {
                ReceiptHeader = "Purchase Receipt/Invoice",
                PurchaseItems = new List<PurchaseItemResponse>
                {
                    new PurchaseItemResponse { ProductName = "Test Product", Quantity = 2, Price = 100 }
                },
                TotalAmount = 200
            };

            _mockRepository.Setup(repo => repo.GeneratePurchaseReceiptInvoiceAsync())
                .ReturnsAsync(receipt);

            // Act
            var result = await _service.GeneratePurchaseReceiptInvoiceAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Purchase Receipt/Invoice", result.ReceiptHeader);
            Assert.AreEqual(1, result.PurchaseItems.Count);
            Assert.AreEqual(200, result.TotalAmount);
            _mockRepository.Verify(repo => repo.GeneratePurchaseReceiptInvoiceAsync(), Times.Once);
        }
    }
}
