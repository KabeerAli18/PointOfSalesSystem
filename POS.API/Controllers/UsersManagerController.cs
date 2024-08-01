using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POS.API.MiddleWares;
using POS.API.MODEL.Users;
using POS.API.SERVICES.UserServices;
using WebApisPointOfSales.Dto.UserDtos;

/// <summary>
/// This is The User ManagementController, Holding end Points for Register User, LOgin Authentication, Change User Roles
/// also the Get all Users End POINTS
/// </summary>
namespace POS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersManagerController : ControllerBase
    {
        private readonly ILogger<UsersManagerController> _logger;
        private readonly IUserManagerService _userManagerService;
        private readonly AuthBearerMiddleware _authService;
        private readonly IMapper _mapper;

        public UsersManagerController(IUserManagerService userManagerService, ILogger<UsersManagerController> logger, AuthBearerMiddleware authService, IMapper mapper)
        {
            _userManagerService = userManagerService;
            _logger = logger;
            _authService = authService;
            _mapper = mapper;
        }

        //For Basic Auth
        //public UsersManagerController(IUserManagerService userManagerService, ILogger<UsersManagerController> logger,IMapper mapper)
        //{
        //    _userManagerService = userManagerService;
        //    _logger = logger;
        //    _mapper = mapper;
        //}

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _logger.LogInformation("Attempting to register user with email: {Email}", request.Email);

                // Map RegisterUserDto to Users entity
                var userEntity = _mapper.Map<Users>(request);

                var registeredUser = await _userManagerService.RegisterUser(userEntity);
                _logger.LogInformation("User registered successfully with email: {Email}", request.Email);
                return Ok("User registered successfully.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error registering user with email: {Email}", request.Email);
                return BadRequest(new { message = ex.Message });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal Server Error Occurred");
                // Log the exception
                return StatusCode(500, new { message = "An unexpected error occurred Now.", details = "Please contact support." });
            }


        }

        [HttpGet("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogInUserAuthentication([FromBody] LoginUserDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _logger.LogInformation("Attempting to log in user with email: {Email}", request.Email);
                var user = await _userManagerService.LogInUserAuthentication(request.Email, request.Password);
                if (user == null)
                {
                    _logger.LogWarning("Invalid login attempt for email: {Email}", request.Email);
                    return Unauthorized("Invalid credentials.");
                }

                _logger.LogInformation("User logged in successfully with email: {Email}", request.Email);
                var token = _authService.GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error logging in user with email: {Email}", request.Email);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal Server Error Occurred");
                // Log the exception
                return StatusCode(500, new { message = "An unexpected error occurred.", details = "Please contact support." });
            }


        }

        [HttpPost("changeRole")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> ChangeUserRole([FromQuery] string email, [FromQuery] string newRole)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _logger.LogInformation("Attempting to change role for user with email: {Email}", email);
                if (await _userManagerService.ChangeUserRole(email, newRole))
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
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal Server Error Occurred");
                // Log the exception
                return StatusCode(500, new { message = "An unexpected error occurred.", details = "User Role can only be Admin or Cashier" });
            }

        }

        [HttpGet("all-users")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> GetAllUsers()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _logger.LogInformation("Attempting to retrieve all users.");
                var users = await _userManagerService.GetAllUsers();
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
