using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSales.Entities
{

    public class Users
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; private set; }

        public string Email { get; private set; }

        public string Password { get; private set; }

        public string UserRole { get; private set; }

        public Users(string name, string email, string password, string userRole)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            UserRole = userRole ?? throw new ArgumentNullException(nameof(userRole));
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
        public void SetUserRole(string userRole)
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
