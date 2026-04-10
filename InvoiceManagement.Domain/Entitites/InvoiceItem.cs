using System.Text.Json.Serialization;

namespace InvoiceManagement.Domain.Entitites
{
    public class InvoiceItem
    {
        public Guid Id { get; set; }

        public Guid InvoiceId { get; set; }

        public string Description { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalItemAmount { get; set; }

        public string? Justification { get; set; }

        [JsonIgnore]
        public Invoice Invoice { get; set; } = null!;
    }
}
