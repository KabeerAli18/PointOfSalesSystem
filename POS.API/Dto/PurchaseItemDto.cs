using System.ComponentModel.DataAnnotations;

namespace WebApisPointOfSales.Dto
{
    public class PurchaseItemDto
    {
        [Required(ErrorMessage = "ProductId is required")]
        public string ProductId { get; set; }=string.Empty;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }
    }
}
