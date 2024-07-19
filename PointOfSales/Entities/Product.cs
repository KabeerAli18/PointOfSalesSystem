using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }

        public Product(int id ,string name, decimal price, int quantity, string type, string category)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            Type = type;
            Category = category;
        }

        public override string ToString()
        {
            return $"Product ID: {Id}, Name: {Name}, Price: {Price}, Quantity: {Quantity}, Type: {Type}, Category: {Category}";
        }
    }

    
}
