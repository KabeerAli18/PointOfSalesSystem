using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.MODEL.Purchase
{
    public class PurchaseItemResponse
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class PurchaseReceiptResponse
    {
        public string ReceiptHeader { get; set; } = string.Empty;
        public List<PurchaseItemResponse> PurchaseItems { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }
}
