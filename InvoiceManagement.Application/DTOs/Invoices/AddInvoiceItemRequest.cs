using System.ComponentModel.DataAnnotations;

namespace InvoiceManagement.Application.DTOs.Invoices
{
    public class AddInvoiceItemRequest
    {
        [Required]
        [MinLength(3)]
        public string Description { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        public string? Justification { get; set; }
    }
}
