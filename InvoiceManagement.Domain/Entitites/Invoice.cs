using InvoiceManagement.Domain.Enums;

namespace InvoiceManagement.Domain.Entitites
{
    public class Invoice
    {
        public Guid Id { get; set; }

        public string Number { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public DateTime IssueDate { get; set; }

        public InvoiceStatus Status { get; set; }

        public decimal TotalAmount { get; set; }

        public List<InvoiceItem> Items { get; set; } = new();

        public void RecalculateTotal()
        {
            TotalAmount = Items.Sum(x => x.TotalItemAmount);
        }

        public void Close()
        {
            Status = InvoiceStatus.Closed;
        }
    }
}
