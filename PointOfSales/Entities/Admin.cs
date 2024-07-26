using System.Text.Json.Serialization;

namespace PointOfSales.Entities
{
    public class Admin : Users
    {
        [JsonConstructor]
        public Admin(string name, string email, string password, string userRole)
            : base(name, email, password, userRole)
        {
        }

        // Parameterless constructor for deserialization
        public Admin() { }
    }
}
