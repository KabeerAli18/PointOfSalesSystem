using System.ComponentModel.DataAnnotations;

namespace WebApisPointOfSales.Dto.UserDtos
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [StringLength(100, MinimumLength = 6)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "UserRole is required")]
        public string UserRole { get; set; } = string.Empty;
    }
}