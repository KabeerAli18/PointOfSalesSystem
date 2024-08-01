using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace POS.API.MODEL.Users
{
    public class Admin : Users
    {
        [JsonConstructor]
        public Admin(string id,string name, string email, string password, string userRole)
            : base(id,name, email, password, userRole)
        {
        }

        // Parameterless constructor for deserialization
        public Admin() { }
    }
}
