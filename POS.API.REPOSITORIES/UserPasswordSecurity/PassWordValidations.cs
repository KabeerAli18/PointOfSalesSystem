using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.REPOSITORIES.UserPasswordSecurity
{
    public class PassWordValidations
    {
        public static bool ValidatePassword(string password)
        {
            if (password == null) throw new NullReferenceException();
            if (password.Length == 0) throw new ArgumentException("Password Length can't be Zero");
            if (password.Length < 12) throw new ArgumentException("Password must be at least 12 characters long");

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            if (!hasUpperCase) throw new ArgumentException("Password must contain at least one uppercase letter");
            if (!hasLowerCase) throw new ArgumentException("Password must contain at least one lowercase letter");
            if (!hasDigit) throw new ArgumentException("Password must contain at least one digit");
            if (!hasSpecialChar) throw new ArgumentException("Password must contain at least one special character");

            if (hasDigit && hasLowerCase && hasUpperCase && hasSpecialChar)
            {
                return true;
            }
            else
            {
                return false;
            }


        }
    }
}
