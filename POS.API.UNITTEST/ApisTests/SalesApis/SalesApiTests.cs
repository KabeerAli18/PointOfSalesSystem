using AutoMapper;
using Azure.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using POS.API.Controllers;
using POS.API.MODEL.Sales;
using POS.API.SERVICES.SaleServices;
using System;
using System.Threading.Tasks;
using WebApisPointOfSales.Controllers;
using WebApisPointOfSales.Dto;

namespace POS.API.Tests
{
    [TestFixture]
    public class SalesTransactionControllerTests
    {
        private Mock<ISalesTransactionService> _salesTransactionServiceMock;
        private Mock<ILogger<SalesTransactionController>> _loggerMock;
        private Mock<IMapper> _mapperMock;
        private SalesTransactionController _controller;

        [SetUp]
        public void Setup()
        {
            _salesTransactionServiceMock = new Mock<ISalesTransactionService>();
            _loggerMock = new Mock<ILogger<SalesTransactionController>>();
            _mapperMock = new Mock<IMapper>();
            _controller = new SalesTransactionController(_salesTransactionServiceMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task AddProductToSale_ShouldReturnOk_WhenValidRequest()
        {
            // Arrange
            var itemDto = new SaleItemDTO { ProductId = "abc", Quantity = 2 };
            var saleItem = new SaleItem { ProductId = "abc", Quantity = 2 };
            _mapperMock.Setup(m => m.Map<SaleItem>(itemDto)).Returns(saleItem);
            _salesTransactionServiceMock.Setup(service => service.AddProductToSaleAsync(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddProductToSale(itemDto) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual("Product added to sale successfully.", result.Value);
        }

        [Test]
        public async Task AddProductToSale_ShouldReturnBadRequest_WhenArgumentExceptionThrown()
        {
            // Arrange
            var itemDto = new SaleItemDTO { ProductId = "abc", Quantity = 2 };
            var saleItem = new SaleItem { ProductId = "abc", Quantity = 2 };
            _mapperMock.Setup(m => m.Map<SaleItem>(itemDto)).Returns(saleItem);
            _salesTransactionServiceMock.Setup(service => service.AddProductToSaleAsync(It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new ArgumentException("Invalid product ID."));

            // Act
            var result = await _controller.AddProductToSale(itemDto) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.AreEqual("Invalid product ID.", result.Value);
        }

    }
}
