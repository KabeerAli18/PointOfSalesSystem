using PointOfSales.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PointOfSales.Interfaces
{
    public interface IUserManagerService
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
