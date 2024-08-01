using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.MODEL.Users
{
    public class Users
    {
        public string id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [StringLength(100, MinimumLength = 6)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "UserRole is required")]
        public string UserRole { get; set; } = string.Empty;

        // Parameterless constructor
        public Users()
        {
        }

        // JsonConstructor
        //[JsonConstructor]
        protected Users(string Id,string name, string email, string password, string userRole)
        {
            Id= id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            UserRole = userRole ?? throw new ArgumentNullException(nameof(userRole));
        }

        public string GetName() => Name;
        public void SetName(string name) => Name = name;
        public string GetEmail() => Email;
        public void SetEmail(string email) => Email = email;
        public string GetPassword() => Password;
        public void SetPassword(string password) => Password = password;
        public void SetUserRole(string userRole) => UserRole = userRole;

        public override string ToString() => $"Name: {Name}, Email: {Email}, UserRole: {UserRole}";
    }
}
