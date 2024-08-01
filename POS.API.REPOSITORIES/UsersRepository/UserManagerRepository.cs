using Microsoft.EntityFrameworkCore;
using POS.API.DATA;
using POS.API.MODEL.Users;
using POS.API.REPOSITORIES.UserPasswordSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace POS.API.REPOSITORIES.UsersRepository
{
    public class UserManagerRepository : IUserManagerRepository
    {
        private readonly MyDbContext _context;
        private static readonly string Key = "b14ca5898a4e4133bbce2ea2315a1916";

        public UserManagerRepository(MyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Users> RegisterUser(Users user)
        {
            try
            {
                if (!IsValidEmail(user.Email))
                {
                    throw new ArgumentException("Invalid email format.");
                }

                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    throw new ArgumentException("User already exists with this email.");
                }

                if (!IsAdminRole(user.UserRole) && !IsCashierRole(user.UserRole))
                {
                    throw new ArgumentException("UserRole must be either 'Admin' or 'Cashier'.");
                }

                bool isPasswordFine = PassWordValidations.ValidatePassword(user.Password);
                if (!isPasswordFine)
                {
                    throw new ArgumentException("Password Requirements are Not Fulfilled.");
                }

                var encryptedPassword = PassWordSecurity.EncryptPassword(Key, user.Password);
                Users newUser = IsAdminRole(user.UserRole)
                    ? new Admin(user.id,user.Name, user.Email, encryptedPassword, "Admin")
                    : new Cashier(user.id, user.Name, user.Email, encryptedPassword, "Cashier");

                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return newUser;
            }
            catch (ArgumentException ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "Error during user registration.");
                throw new ArgumentException(ex.Message);// Re-throw the exception to be handled by the caller if necessary
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "An unexpected error occurred during user registration.");
                throw new Exception("An unexpected error occurred. Please try again later.", ex);
            }
        }


        public async Task<Users> LogInUserAuthentication(string email, string password)
        {
            try
            {
                bool isPasswordFine = PassWordValidations.ValidatePassword(password);
                if (isPasswordFine)
                {
                    var encryptedPassword = PassWordSecurity.EncryptPassword(Key, password);
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
            catch (ArgumentException ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "Error during user login authentication.");
                throw new ArgumentException(ex.Message); // Re-throw the exception to be handled by the caller if necessary
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "An unexpected error occurred during user login authentication.");
                throw new Exception("An unexpected error occurred. Please try again later.", ex);
            }
        }


        public async Task<bool> ChangeUserRole(string email, string newRole)
        {
            try
            {
                if (!IsAdminRole(newRole) && !IsCashierRole(newRole))
                {
                    throw new ArgumentException("New role must be either 'Admin' or 'Cashier'.");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                if (user != null)
                {
                    user.SetUserRole(newRole);
                    await _context.SaveChangesAsync();
                    return true;
                    //if (IsAdminRole(user.UserRole)) // Check if the current user is an Admin
                    //{
                    //    if (IsAdminRole(newRole))
                    //    {
                    //        user.SetUserRole("Admin");
                    //    }
                    //    else
                    //    {
                    //        user.SetUserRole("Cashier");
                    //    }
                    //    await _context.SaveChangesAsync();
                    //    return true;
                    //}
                    //else
                    //{
                    //    throw new ArgumentException("Only Admin can update the roles.");
                    //}
                }
                return false;
            }
            catch (ArgumentException ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "Error during role change.");
                throw new ArgumentException(ex.Message); // Re-throw the exception to be handled by the caller if necessary
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "An unexpected error occurred during role change.");
                throw new Exception("An unexpected error occurred. Please try again later.", ex);
            }
        }
        public async Task<IEnumerable<Users>> GetAllUsers()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "An unexpected error occurred while retrieving users.");
                throw new Exception("An unexpected error occurred. Please try again later.", ex);
            }
        }


        public bool IsAdmin(Users user)
        {
            return user.UserRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsCashier(Users user)
        {
            return user.UserRole.Equals("Cashier", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsAdminRole(string role)
        {
            return role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsCashierRole(string role)
        {
            return role.Equals("Cashier", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsValidEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }


    }
}
