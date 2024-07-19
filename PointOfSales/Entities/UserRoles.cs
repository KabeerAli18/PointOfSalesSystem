using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSales.Entities
{
    public class UserRoles
    {
        public string RoleName { get; set; } = string.Empty;
    }

    public static class Roles
    {
        public static readonly UserRoles Admin = new UserRoles { RoleName = "Admin" };
        public static readonly UserRoles Cashier = new UserRoles { RoleName = "Cashier" };
    }
}
