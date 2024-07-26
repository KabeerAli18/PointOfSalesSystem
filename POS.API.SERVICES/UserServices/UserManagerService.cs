using POS.API.MODEL.Users;
using POS.API.REPOSITORIES.UsersRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS.API.SERVICES.UserServices
{
    public class UserManagerService : IUserManagerService
    {
        private readonly IUserManagerRepository _userManagerRepository;

        public UserManagerService(IUserManagerRepository userManagerRepository)
        {
            _userManagerRepository = userManagerRepository;
        }

        public async Task<Users> RegisterUser(Users user)
        {
            try
            {
                var newUser = await _userManagerRepository.RegisterUser(user);
                return newUser;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while Registering User: ", ex);
            }
        }

        public async Task<Users> LogInUserAuthentication(string email, string password)
        {
            try
            {
                var user = await _userManagerRepository.LogInUserAuthentication(email, password);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while Authenticating User: ", ex);
            }
        }

        public async Task<bool> ChangeUserRole(string email, string newRole)
        {
            try
            {
                var result = await _userManagerRepository.ChangeUserRole(email, newRole);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while Changing User Role: ", ex);
            }
        }

        public async Task<IEnumerable<Users>> GetAllUsers()
        {
            try
            {
                var users = await _userManagerRepository.GetAllUsers();
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while Retrieving All Users: ", ex);
            }
        }

        public bool IsAdmin(Users user)
        {
            try
            {
                return _userManagerRepository.IsAdmin(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while Checking if User is Admin: ", ex);
            }
        }

        public bool IsCashier(Users user)
        {
            try
            {
                return _userManagerRepository.IsCashier(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while Checking if User is Cashier: ", ex);
            }
        }

        public bool IsAdminRole(string role)
        {
            try
            {
                return _userManagerRepository.IsAdminRole(role);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while Checking if Role is Admin: ", ex);
            }
        }
        public bool IsCashierRole(string role)
        {
            try
            {
                return _userManagerRepository.IsCashierRole(role);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while Checking if Role is Cashier: ", ex);
            }
        }
    }
}
