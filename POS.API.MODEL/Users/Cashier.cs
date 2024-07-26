using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace POS.API.MODEL.Users
{
    public class Cashier : Users
    {
        [JsonConstructor]
        public Cashier(string name, string email, string password, string userRole)
            : base(name, email, password, userRole)
        {
        }

        // Parameterless constructor for deserialization
        public Cashier() { }
    }
}
