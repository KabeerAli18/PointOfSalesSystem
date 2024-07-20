using System;

namespace PointOfSales.Entities
{
    public class PurchaseItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        // public decimal TotalPrice { get; set; }
        public Product Product { get; set; }
        public PurchaseItem()
        {
            
        }
        public PurchaseItem(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
