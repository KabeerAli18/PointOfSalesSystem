using POS.API.MODEL.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.MODEL.Sales
{
    public class SaleItem
    {
        public string id { get; set; } // Primary key
        [Required(ErrorMessage = "ProductId is required")]
        public string ProductId { get; set; }

        [Required(ErrorMessage = "SalesItemName is required")]
        [StringLength(100, ErrorMessage = "SalesItemName length can't be more than 100 characters.")]
        public string SalesItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
        public Product Product { get; set; }

        public SaleItem()
        {
            Product = new Product(); // Initialize to avoid null reference
        }

        public SaleItem(string Id,string productId, int quantity, decimal price, string name)
        {
            id = Id;
            ProductId = productId;
            SalesItemName = name;
            Quantity = quantity;
            Price = price;
            Product = new Product(); // Initialize to avoid null reference
        }
    }
}
