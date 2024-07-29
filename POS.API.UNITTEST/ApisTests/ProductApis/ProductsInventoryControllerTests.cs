using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApisPointOfSales.Controllers;
using POS.API.SERVICES.ProductServices;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using WebApisPointOfSales.Dto.ProductsDtos;
using POS.API.MODEL.Products;

namespace POS.API.UNITTEST.ApisTests.ProductApis
{
    [TestFixture]
    public class ProductsInventoryControllerTests
    {
        private ProductsInventoryController _controller;
        private Mock<IInventoryManagerService> _mockService;
        private Mock<ILogger<ProductsInventoryController>> _mockLogger;
        private Mock<IMapper> _mockMapper;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IInventoryManagerService>();
            _mockLogger = new Mock<ILogger<ProductsInventoryController>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new ProductsInventoryController(_mockService.Object, _mockLogger.Object, _mockMapper.Object);
        }

        [Test]
        public async Task AddProduct_ValidData_ReturnsOk()
        {
            // Arrange
            var productDto = new CreateProductDto
            {
                Name = "ABCBuuuiiGu",
                Price = 89,
                Quantity = 90,
                Type = "ABYC",
                Category = "string"
            };

            _mockMapper.Setup(m => m.Map<Product>(It.IsAny<CreateProductDto>())).Returns(new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                Quantity = productDto.Quantity,
                Type = productDto.Type,
                Category = productDto.Category
            });

            _mockService.Setup(s => s.AddProductAsync(It.IsAny<Product>())).ReturnsAsync(new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                Quantity = productDto.Quantity,
                Type = productDto.Type,
                Category = productDto.Category
            });

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.User = TestHelper.GetTestUser("Admin");

            // Act
            var result = await _controller.AddProduct(productDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo("Product added successfully."));
        }

        //[Test]
        //public async Task AddProduct_NonAdminUser_ReturnsForbidden()
        //{
        //    // Arrange
        //    var productDto = new CreateProductDto
        //    {
        //        Name = "ABCBuuuiiGu",
        //        Price = 89,
        //        Quantity = 90,
        //        Type = "ABYC",
        //        Category = "string"
        //    };

        //    _mockMapper.Setup(m => m.Map<Product>(It.IsAny<CreateProductDto>())).Returns(new Product
        //    {
        //        Name = productDto.Name,
        //        Price = productDto.Price,
        //        Quantity = productDto.Quantity,
        //        Type = productDto.Type,
        //        Category = productDto.Category
        //    });

        //    _mockService.Setup(s => s.AddProductAsync(It.IsAny<Product>())).ReturnsAsync(new Product
        //    {
        //        Name = productDto.Name,
        //        Price = productDto.Price,
        //        Quantity = productDto.Quantity,
        //        Type = productDto.Type,
        //        Category = productDto.Category
        //    });

        //    _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        //    _controller.ControllerContext.HttpContext.User = TestHelper.GetTestUser("Cashier"); // Non-Admin role

        //    // Act
        //    var result = await _controller.AddProduct(productDto);

        //    // Assert
        //    Assert.That(result, Is.InstanceOf<ForbidResult>());
        //}
    }

    public static class TestHelper
    {
        public static System.Security.Claims.ClaimsPrincipal GetTestUser(string role)
        {
            var claims = new[] {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role)
            };
            var identity = new System.Security.Claims.ClaimsIdentity(claims, "TestAuthScheme");
            return new System.Security.Claims.ClaimsPrincipal(identity);
        }
    }
}
