using System;
using System.Linq;
using System.Text.RegularExpressions;
using PointOfSales.Entities;
using Microsoft.EntityFrameworkCore;

namespace PointOfSales.Services
{
    public static class UserManager
    {
        private static MyDbContext _context = null!;
        private static readonly string Key = "b14ca5898a4e4133bbce2ea2315a1916";

        public static void Initialize(MyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Register a new user
        public static void RegisterUser(string name, string email, string password, string role)
        {
            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email format.");
            }
            if (_context.Users.Any(u => u.Email == email))
            {
                throw new ArgumentException("User already exists with this email.");
            }

            bool isPasswordFine = PassWordValidatorHandler.ValidatePassword(password);
            if (isPasswordFine)
            {
                var encryptedPassword = PasswordSecurityHandler.EncryptPassword(Key, password);
                var newUser = new Users(name, email, encryptedPassword, role);
                _context.Users.Add(newUser);
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Password Requirements are Not Fulfilled.");
            }
        }

        // Log in a user
        public static Users LogInUserAuthentication(string email, string password)
        {
            bool isPasswordFine = PassWordValidatorHandler.ValidatePassword(password);
            if (isPasswordFine)
            {
                var encryptedPassword = PasswordSecurityHandler.EncryptPassword(Key, password);
                var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == encryptedPassword);
                if (user == null)
                {
                    throw new ArgumentException("Invalid email or password.");
                }
                return user;
            }
            else
            {
                throw new ArgumentException("Password Requirements are Not Fulfilled.");
            }
        }

        // Check if a user is an admin
        public static bool IsAdmin(Users user)
        {
            return user.UserRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        // Check if a user is a cashier
        public static bool IsCashier(Users user)
        {
            return user.UserRole.Equals("Cashier", StringComparison.OrdinalIgnoreCase);
        }

        // Validate email format
        private static bool IsValidEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        // Change user role
        public static bool ChangeUserRole(string email, string newRole)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (user != null)
            {
                user.SetUserRole(newRole);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
