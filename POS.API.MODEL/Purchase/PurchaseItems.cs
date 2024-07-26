using POS.API.MODEL.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.MODEL.Purchase
{
    public class PurchaseItem
    {
        public int Id { get; set; } // Primary key
        [Required(ErrorMessage = "ProductId is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "PurchaseItemName is required")]
        [StringLength(100, ErrorMessage = "PurchaseItemName length can't be more than 100 characters.")]
        public string PurchaseItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
        public Product Product { get; set; }

        public PurchaseItem()
        {
            Product = new Product(); // Initialize to avoid null reference
        }

        public PurchaseItem(int productId, int quantity, decimal price, string name)
        {
            ProductId = productId;
            PurchaseItemName = name;
            Quantity = quantity;
            Price = price;
            Product = new Product(); // Initialize to avoid null reference
        }
    }
}
