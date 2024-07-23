using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PointOfSales;
using PointOfSales.Entities;
using PointOfSales.Services;
using WebApisPointOfSales.Dto;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebApisPointOfSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersManagerController : ControllerBase
    {
        private readonly ILogger<UsersManagerController> _logger;
        private readonly MyDbContext _context;

        public UsersManagerController(MyDbContext context, ILogger<UsersManagerController> logger)
        {
            _context = context;
            _logger = logger;
            UserManager.Initialize(context);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] Users user)
        {
            try
            {
                _logger.LogInformation("Attempting to register user with email: {Email}", user.Email);
                var registeredUser = await UserManager.RegisterUser(user);
                _logger.LogInformation("User registered successfully with email: {Email}", user.Email);
                return Ok("User registered successfully.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error registering user with email: {Email}", user.Email);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogInUserAuthentication([FromBody] LoginDto request)
        {
            try
            {
                _logger.LogInformation("Attempting to log in user with email: {Email}", request.Email);
                var user = await UserManager.LogInUserAuthentication(request.Email, request.Password);
                _logger.LogInformation("User logged in successfully with email: {Email}", request.Email);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error logging in user with email: {Email}", request.Email);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("changeRole")]
        public async Task<IActionResult> ChangeUserRole([FromQuery] string email, [FromQuery] string currentRole, [FromQuery] string newRole)
        {
            try
            {
                _logger.LogInformation("Attempting to change role for user with email: {Email}", email);
                if (await UserManager.ChangeUserRole(email, currentRole, newRole))
                {
                    _logger.LogInformation("User role updated successfully for email: {Email}", email);
                    return Ok("User role updated successfully.");
                }
                _logger.LogWarning("User not found with email: {Email}", email);
                return NotFound("User not found.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error changing role for user with email: {Email}", email);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all users.");
                var users = await UserManager.GetAllUsers();
                _logger.LogInformation("Successfully retrieved all users.");
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}
