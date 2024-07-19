using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POS
{
    public static class PurchaseTransactions
    {
        private static List<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

        public static void AddProductToPurchaseOrder(Product product, int quantity)
        {
            InventoryManager.DisplayInventoryTable();
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

            // Add the product to the purchase order
            var purchaseItem = new PurchaseItem(product, quantity);
            PurchaseItems.Add(purchaseItem);
            InventoryManager.TrackInventory();
        }

        public static decimal CalculateTotalPurchaseAmount()
        {
            return PurchaseItems.Sum(item => item.TotalPrice);
        }

        public static string GeneratePurchaseReceipt()
        {
            var receipt = new StringBuilder();
            receipt.AppendLine("Purchase Receipt");
            receipt.AppendLine("-------------------------------");

            foreach (var item in PurchaseItems)
            {
                receipt.AppendLine($"{item.Product.Name} x {item.Quantity} = {item.TotalPrice:C}");
            }

            receipt.AppendLine("-------------------------------");
            receipt.AppendLine($"Total: {CalculateTotalPurchaseAmount():C}");

            return receipt.ToString();
        }

        // Clear the purchase items after generating the receipt
        public static void ClearPurchaseItems()
        {
            PurchaseItems.Clear();
        }
    }
}
