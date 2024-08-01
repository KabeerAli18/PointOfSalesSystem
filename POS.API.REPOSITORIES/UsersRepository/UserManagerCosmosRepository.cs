using Microsoft.Azure.Cosmos;
using POS.API.MODEL.Users;
using POS.API.REPOSITORIES.UserPasswordSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace POS.API.REPOSITORIES.UsersRepository
{
    public class UserManagerCosmosRepository : IUserManagerRepository
    {
        private readonly Container _container;
        private static readonly string Key = "b14ca5898a4e4133bbce2ea2315a1916";

        public UserManagerCosmosRepository(CosmosClient dbClient, string databaseName, string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<Users> RegisterUser(Users user)
        {
            try
            {
                if (!IsValidEmail(user.Email))
                {
                    throw new ArgumentException("Invalid email format.");
                }

                var existingUser = await GetUserByEmail(user.Email);
                if (existingUser != null)
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
                    throw new ArgumentException("Password requirements are not fulfilled.");
                }

                var encryptedPassword = PassWordSecurity.EncryptPassword(Key, user.Password);
                user.Password = encryptedPassword;

                // Ensure the Id is set
                user.id = Guid.NewGuid().ToString();

                var response = await _container.CreateItemAsync(user, new PartitionKey(user.Email));
                return response.Resource;
            }
            catch (ArgumentException ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error: {ex.Message}");
                throw new ArgumentException(ex.Message); // Re-throw the exception to be handled by the caller if necessary
            }
            catch (CosmosException ex)
            {
                // Log the specific details of the CosmosException
                Console.WriteLine($"CosmosDB error: {ex.StatusCode} - {ex.Message}");
                throw new ArgumentException("CosmosDB error occurred. Please try again later here.", ex);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new ArgumentException("An unexpected error occurred. Please try again later now.", ex);
            }
        }


        public async Task<Users> LogInUserAuthentication(string email, string password)
        {
            try
            {
                var user = await GetUserByEmail(email);
                if (user == null)
                {
                    throw new ArgumentException("Invalid email or password.");
                }

                bool isPasswordFine = PassWordValidations.ValidatePassword(password);
                if (!isPasswordFine)
                {
                    throw new ArgumentException("Password requirements are not fulfilled.");
                }

                var encryptedPassword = PassWordSecurity.EncryptPassword(Key, password);
                if (user.Password != encryptedPassword)
                {
                    throw new ArgumentException("Invalid email or password.");
                }

                return user;
            }
            catch (ArgumentException ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "Error during user login authentication.");
                throw new ArgumentException(ex.Message); // Re-throw the exception to be handled by the caller if necessary
            }
            catch (CosmosException ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "CosmosDB error during user login authentication.");
                throw new ArgumentException("CosmosDB error occurred. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "An unexpected error occurred during user login authentication.");
                throw new ArgumentException("An unexpected error occurred. Please try again later.", ex);
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

                var user = await GetUserByEmail(email);
                if (user != null)
                {
                    user.UserRole = newRole;
                    var response = await _container.UpsertItemAsync(user, new PartitionKey(user.Email));
                    return response.StatusCode == System.Net.HttpStatusCode.OK;
                }

                return false;
            }
            catch (ArgumentException ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "Error during role change.");
                throw new ArgumentException(ex.Message); // Re-throw the exception to be handled by the caller if necessary
            }
            catch (CosmosException ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "CosmosDB error during role change.");
                throw new Exception("CosmosDB error occurred. Please try again later.", ex);
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
                var query = "SELECT * FROM c";
                var iterator = _container.GetItemQueryIterator<Users>(new QueryDefinition(query));
                var results = new List<Users>();
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    results.AddRange(response);
                }
                return results;
            }
            catch (CosmosException ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "CosmosDB error while retrieving users.");
                throw new Exception("CosmosDB error occurred. Please try again later.", ex);
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

        private async Task<Users> GetUserByEmail(string email)
        {
            try
            {
                var query = new QueryDefinition("SELECT * FROM c WHERE c.Email = @Email")
                    .WithParameter("@Email", email);
                var iterator = _container.GetItemQueryIterator<Users>(query);
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    if (response.Resource.Any())
                    {
                        return response.Resource.First();
                    }
                }
                return null;
            }
            catch (CosmosException ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "CosmosDB error while retrieving user by email.");
                throw new Exception("CosmosDB error occurred. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // logger.LogError(ex, "An unexpected error occurred while retrieving user by email.");
                throw new Exception("An unexpected error occurred. Please try again later.", ex);
            }
        }
    }
}
