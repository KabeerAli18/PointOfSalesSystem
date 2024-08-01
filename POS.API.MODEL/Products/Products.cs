using System.ComponentModel.DataAnnotations;

namespace POS.API.MODEL.Products
{
    public class Product
    {
        public string id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name length can't be more than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative integer.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [StringLength(50, ErrorMessage = "Type length can't be more than 50 characters.")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [StringLength(50, ErrorMessage = "Category length can't be more than 50 characters.")]
        public string Category { get; set; } = string.Empty;

        public Product(string Id, string name, decimal price, int quantity, string type, string category)
        {
            id = Id;
            Name = name;
            Price = price;
            Quantity = quantity;
            Type = type;
            Category = category;
        }

        public Product()
        {
        }

        public override string ToString()
        {
            return $"Product ID: {id}, Name: {Name}, Price: {Price}, Quantity: {Quantity}, Type: {Type}, Category: {Category}";
        }
    }
}
