using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSales.Entities
{

    public class Users
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public UserRoles UserRole { get; private set; }

        public Users(string name, string email, string password, UserRoles userRole)
        {
            this.Name = name;
            this.Email = email;
            this.Password = password;
            this.UserRole = userRole;
        }

        public string GetName()
        {
            return Name;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public string GetEmail()
        {
            return Email;
        }

        public void SetEmail(string email)
        {
            Email = email;
        }

        public string GetPassword()
        {
            return Password;
        }

        public void SetPassword(string password)
        {
            Password = password;
        }
        public void SetUserRole(UserRoles userRole)
        {
            UserRole = userRole;
        }

        public override string ToString()
        {
            return $"Name: {Name}, Email: {Email}";
        }

        #region Methods

       



        #endregion
    }

}
