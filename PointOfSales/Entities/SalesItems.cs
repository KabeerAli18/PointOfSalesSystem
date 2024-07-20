using System;

namespace PointOfSales.Entities
{
    public class SaleItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        //public decimal TotalPrice { get; set; } 

        public Product Product { get; set; }
        public SaleItem()
        {
            
        }
        public SaleItem(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
