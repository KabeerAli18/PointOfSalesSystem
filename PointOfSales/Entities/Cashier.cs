using System.Text.Json.Serialization;

namespace PointOfSales.Entities
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
