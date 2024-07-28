using NUnit.Framework;
using Moq;
using POS.API.Controllers;
using POS.API.SERVICES.UserServices;
using WebApisPointOfSales.Dto.UserDtos;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using POS.API.MiddleWares;
using POS.API.MODEL.Users;
using POS.API.UNITTEST.ApisTests;

namespace POS.API.UNITTEST.ApisTests.UserApis
{
    [TestFixture]
    public class UserApiTests
    {
        private Mock<IUserManagerService> _userManagerServiceMock;
        private Mock<ILogger<UsersManagerController>> _loggerMock;
        private Mock<IMapper> _mapperMock;
        private UsersManagerController _controller;

        [SetUp]
        public void Setup()
        {
            _userManagerServiceMock = new Mock<IUserManagerService>();
            _loggerMock = new Mock<ILogger<UsersManagerController>>();
            _mapperMock = new Mock<IMapper>();

            var mockAuthService = new MockAuthBearerMiddleware();

            _controller = new UsersManagerController(_userManagerServiceMock.Object, _loggerMock.Object, mockAuthService, _mapperMock.Object);
        }

        [Test]
        public async Task RegisterUser_ShouldReturnOk_WhenValidRequest()
        {
            // Arrange
            var userDto = new RegisterUserDto { Name = "Test User", Email = "test@example.com", Password = "Password123#", UserRole = "Admin" };
            var user = new Users { Name = "Test User", Email = "test@example.com", Password = "Password123#", UserRole = "Admin" };
            _mapperMock.Setup(m => m.Map<Users>(userDto)).Returns(user);
            _userManagerServiceMock.Setup(service => service.RegisterUser(It.IsAny<Users>())).ReturnsAsync(user);

            // Act
            var result = await _controller.RegisterUser(userDto) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("User registered successfully.", result.Value);
        }



        [Test]
        public async Task RegisterUser_ShouldReturnBadRequest_WhenInvalidEmail()
        {
            // Arrange
            var userDto = new RegisterUserDto { Name = "Test User", Email = "invalid", Password = "Password123#", UserRole = "Admin" };
            _mapperMock.Setup(m => m.Map<Users>(userDto)).Returns(It.IsAny<Users>());
            _userManagerServiceMock.Setup(service => service.RegisterUser(It.IsAny<Users>()))
                .ThrowsAsync(new ArgumentException("Invalid email format."));

            // Act
            var result = await _controller.RegisterUser(userDto) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        // Add more tests for other scenarios
    }
}
