using Microsoft.EntityFrameworkCore;
using InvoiceManagement.Application.DTOs.Invoices;
using InvoiceManagement.Domain.Entitites;
using InvoiceManagement.Domain.Enums;
using InvoiceManagement.Domain.Exceptions;
using InvoiceManagement.Infrastructure.Persistence;

namespace InvoiceManagement.Application.Services
{
    public class InvoiceService
    {
        private readonly AppDbContext _context;

        public InvoiceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice> CreateAsync(CreateInvoiceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CustomerName))
                throw new BusinessException("Customer name is required.");

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                Number = $"INV-{DateTime.Now:yyyyMMddHHmmss}",
                CustomerName = request.CustomerName,
                IssueDate = DateTime.UtcNow,
                Status = InvoiceStatus.Open,
                TotalAmount = 0
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return invoice;
        }

        public async Task AddItemAsync(Guid invoiceId, AddInvoiceItemRequest request)
        {
            var invoice = await _context.Invoices
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == invoiceId);

            if (invoice == null)
                throw new BusinessException("Invoice not found.");

            if (invoice.Status == InvoiceStatus.Closed)
                throw new BusinessException("Closed invoice cannot be changed.");

            if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Length < 3)
                throw new BusinessException("Description must have at least 3 characters.");

            var total = request.Quantity * request.UnitPrice;

            if (total > 1000 && string.IsNullOrWhiteSpace(request.Justification))
                throw new BusinessException("Justification is required for items above R$ 1,000.");

            var item = new InvoiceItem
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoiceId,
                Description = request.Description,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                TotalItemAmount = total,
                Justification = request.Justification
            };

            _context.InvoiceItems.Add(item);

            invoice.TotalAmount += total;

            _context.Invoices.Update(invoice);

            await _context.SaveChangesAsync();
        }

        public async Task CloseAsync(Guid invoiceId)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);

            if (invoice == null)
                throw new BusinessException("Invoice not found.");

            if (invoice.Status == InvoiceStatus.Closed)
                throw new BusinessException("Invoice is already closed.");

            invoice.Close();

            await _context.SaveChangesAsync();
        }

        public async Task<Invoice?> GetByIdAsync(Guid id)
        {
            return await _context.Invoices
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Invoice>> GetAllAsync(
            string? customer,
            DateTime? startDate,
            DateTime? endDate,
            string? status)
        {
            var query = _context.Invoices
                .Include(x => x.Items)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(customer))
                query = query.Where(x => x.CustomerName.Contains(customer));

            if (startDate.HasValue)
                query = query.Where(x => x.IssueDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(x => x.IssueDate <= endDate.Value);

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<InvoiceStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(x => x.Status == parsedStatus);
            }

            return await query.ToListAsync();
        }
    }
}
