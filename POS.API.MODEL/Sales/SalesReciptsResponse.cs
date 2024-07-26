using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.MODEL.Sales
{
    public class SaleItemResponse
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class SalesReceiptResponse
    {
        public string ReceiptHeader { get; set; } = string.Empty;
        public List<SaleItemResponse> SaleItems { get; set; } = new List<SaleItemResponse>();
        public decimal TotalAmount { get; set; }
    }
}
