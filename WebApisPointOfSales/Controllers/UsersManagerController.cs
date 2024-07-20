using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PointOfSales;
using PointOfSales.Entities;
using PointOfSales.Services;
using WebApisPointOfSales.Dto;
using System;
using Microsoft.AspNetCore.Identity.Data;

namespace WebApisPointOfSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersManagerController : ControllerBase
    {
        public UsersManagerController(MyDbContext context)
        {
            UserManager.Initialize(context);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] Users user)
        {
            try
            {
                var registeredUser = await UserManager.RegisterUser(user);
                return Ok("User registered successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogInUserAuthentication([FromBody] LoginDto request)
        {
            try
            {
                var user = await UserManager.LogInUserAuthentication(request.Email, request.Password);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("changeRole")]
        public async Task<IActionResult> ChangeUserRole([FromQuery] string email, [FromQuery] string currentRole, [FromQuery] string newRole)
        {
            try
            {
                if (await UserManager.ChangeUserRole(email, currentRole, newRole))
                {
                    return Ok("User role updated successfully.");
                }
                return NotFound("User not found.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await UserManager.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


    }
}
