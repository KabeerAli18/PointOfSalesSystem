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
        public static async Task<Users> RegisterUser(Users user)
        {
            if (!IsValidEmail(user.Email))
            {
                throw new ArgumentException("Invalid email format.");
            }
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                throw new ArgumentException("User already exists with this email.");
            }
            if (!IsAdmin(user) && !IsCashier(user))
            {
                throw new ArgumentException("UserRole must be either 'Admin' or 'Cashier'.");
            }

            bool isPasswordFine = PassWordValidatorHandler.ValidatePassword(user.Password);
            if (isPasswordFine)
            {
                var encryptedPassword = PasswordSecurityHandler.EncryptPassword(Key, user.Password);
                var newUser = new Users(user.Name, user.Email, encryptedPassword, user.UserRole);
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return newUser;
            }
            else
            {
                throw new ArgumentException("Password Requirements are Not Fulfilled.");
            }
        }

        // Log in a user
        public static async Task<Users> LogInUserAuthentication(string email, string password)
        {
            bool isPasswordFine = PassWordValidatorHandler.ValidatePassword(password);
            if (isPasswordFine)
            {
                var encryptedPassword = PasswordSecurityHandler.EncryptPassword(Key, password);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == encryptedPassword);
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

        //Change the User Role
        public static async Task<bool> ChangeUserRole(string email, string currentRole, string newRole)
        {
            // Validate the new role
            if (!IsAdminRole(newRole) && !IsCashierRole(newRole))
            {
                throw new ArgumentException("New role must be either 'Admin' or 'Cashier'.");
            }

            // Find the user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (user != null)
            {
                // Check if the current user role is Admin
                if (IsAdminRole(currentRole))
                {
                    user.SetUserRole(newRole);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    throw new ArgumentException("Only Admin can update the roles.");
                }
            }
            return false; // Return false if user is not found
        }

        //Get All Users
        public static async Task<IEnumerable<Users>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
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

        public static bool IsAdminRole(string role)
        {
            return role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        // Check if a role is cashier
        public static bool IsCashierRole(string role)
        {
            return role.Equals("Cashier", StringComparison.OrdinalIgnoreCase);
        }
        // Validate email format
        private static bool IsValidEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

       
    }
}
