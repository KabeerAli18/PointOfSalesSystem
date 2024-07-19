using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PointOfSales.Entities;
namespace PointOfSales.Services
{
    public static class UserManager
    {
        private static List<Users> users = new List<Users>();
        //{
        //    new Users ("Kabeer","kab@gmail.com","Kabeer000000#",new Roles.Admin="Admin"),
        //    new Users ("Bilal","bilal@gmail.com","Bilal000000#","Cashier")

        //};
        private static readonly string Key = "b14ca5898a4e4133bbce2ea2315a1916";

        // Register a new user
        public static void RegisterUser(string name, string email, string password, UserRoles role)
        {
            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email format.");
            }
            if (users.Any(u => u.Email == email))
            {
                throw new ArgumentException("User already exists with this email.");
            }

            bool isPasswordFine = PassWordValidatorHandler.ValidatePassword(password);
            if (isPasswordFine)
            {
                var encryptedPassword = PasswordSecurityHandler.EncryptPassword(Key, password);
                var newUser = new Users(name, email, encryptedPassword, role);
                users.Add(newUser);
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
                var user = users.FirstOrDefault(u => u.Email == email && u.Password == encryptedPassword);
                if (user == null)
                {
                    throw new ArgumentException("Invalid email or password.");
                }
                return user;
            }
            else
            {
                throw new ArgumentException("User is not Authenticated Due to Password Requirements Not Being Fulfilled.");
            }
        }

        // Check if a user is an admin
        public static bool IsAdmin(Users user)
        {
            return user.UserRole.RoleName == Roles.Admin.RoleName;
        }

        // Check if a user is a cashier
        public static bool IsCashier(Users user)
        {
            return user.UserRole.RoleName == Roles.Cashier.RoleName;
        }

        // Validate email format
        private static bool IsValidEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        // Change user role
        public static bool ChangeUserRole(string email, UserRoles newRole)
        {
            var user = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (user != null)
            {
                user.SetUserRole(newRole);
                return true;
            }
            return false;
        }
    }
}
