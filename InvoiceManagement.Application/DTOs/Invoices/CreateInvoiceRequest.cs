using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceManagement.Application.DTOs.Invoices
{
    public class CreateInvoiceRequest
    {
        [Required]
        [MinLength(3)]
        public string CustomerName { get; set; } = string.Empty;
    }
}
