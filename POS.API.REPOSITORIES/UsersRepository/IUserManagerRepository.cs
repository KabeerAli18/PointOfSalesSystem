using POS.API.MODEL.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.REPOSITORIES.UsersRepository
{
    public interface IUserManagerRepository
    {
        Task<Users> RegisterUser(Users user);
        Task<Users> LogInUserAuthentication(string email, string password);
        Task<bool> ChangeUserRole(string email, string newRole);
        Task<IEnumerable<Users>> GetAllUsers();
        bool IsAdmin(Users user);
        bool IsCashier(Users user);
        bool IsAdminRole(string role);
        bool IsCashierRole(string role);
    }
}
