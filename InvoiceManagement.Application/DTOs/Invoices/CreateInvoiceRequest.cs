using System.ComponentModel.DataAnnotations;

namespace InvoiceManagement.Application.DTOs.Invoices
{
    public class CreateInvoiceRequest
    {
        [Required]
        [MinLength(3)]
        public string CustomerName { get; set; } = string.Empty;
    }
}
