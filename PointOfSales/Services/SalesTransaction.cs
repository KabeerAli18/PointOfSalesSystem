using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POS
{
    public static class SalesTransaction
    {
        private static List<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

        public static void AddProductToSale(Product product, int quantity)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            // Find the product in inventory
            var prod = InventoryManager.FindProductByID(product.Id);
            if (prod == null)
            {
                throw new InvalidOperationException("Product not found in inventory.");
            }

            // Check if the product has enough quantity
            if (prod.Quantity < quantity)
            {
                throw new InvalidOperationException("Insufficient quantity in stock.");
            }

            // Reduce the product's quantity
            prod.Quantity -= quantity;

            // Add the product to the sale
            var saleItem = new SaleItem(product, quantity);
            SaleItems.Add(saleItem);
            InventoryManager.TrackInventory();
        }



        public static decimal CalculateTotalSalesAmount()
        {
            return SaleItems.Sum(item => item.TotalPrice);
        }

        public static string GenerateSalesTransactionsReceipt()
        {
            var receipt = new StringBuilder();
            receipt.AppendLine("Sales Transaction Receipt");
            receipt.AppendLine("-------------------------------");

            foreach (var item in SaleItems)
            {
                receipt.AppendLine($"{item.Product.Id}: {item.Product.Name} x {item.Product.Quantity} = {item.TotalPrice:C}");
            }

            receipt.AppendLine("-------------------------------");
            receipt.AppendLine($"Total: {CalculateTotalSalesAmount()}:C");

            return receipt.ToString();
        }

        public static void ClearSaleItems()
        {
            SaleItems.Clear();
        }
    }
}
