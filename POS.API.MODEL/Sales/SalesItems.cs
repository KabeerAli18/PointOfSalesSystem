using POS.API.MODEL.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.MODEL.Sales
{
    public class SaleItem
    {
        public int Id { get; set; } // Primary key
        public int ProductId { get; set; }
        public string SalesItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Product Product { get; set; }

        public SaleItem()
        {
            Product = new Product(); // Initialize to avoid null reference
        }

        public SaleItem(int productId, int quantity, decimal price, string name)
        {
            ProductId = productId;
            SalesItemName = name;
            Quantity = quantity;
            Price = price;
            Product = new Product(); // Initialize to avoid null reference
        }
    }
}
