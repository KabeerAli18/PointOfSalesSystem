using POS.API.MODEL.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.MODEL.Purchase
{
    public class PurchaseItem
    {
        public int Id { get; set; } // Primary key
        public int ProductId { get; set; }
        public string PurchaseItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
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
