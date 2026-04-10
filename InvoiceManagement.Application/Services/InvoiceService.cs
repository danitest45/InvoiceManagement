using InvoiceManagement.Application.DTOs.Invoices;
using InvoiceManagement.Application.Interfaces;
using InvoiceManagement.Domain.Entitites;
using InvoiceManagement.Domain.Enums;
using InvoiceManagement.Domain.Exceptions;
using InvoiceManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InvoiceManagement.Application.Services
{
    public class InvoiceService(AppDbContext context, ILogger<InvoiceService> logger) : IInvoiceService
    {
        private readonly AppDbContext _context = context;
        private readonly ILogger<InvoiceService> _logger = logger;

        public async Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CustomerName))
                throw new BusinessException("Customer name is required.");

            _logger.LogInformation("Creating invoice for customer {CustomerName}", request.CustomerName);

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                Number = $"INV-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
                CustomerName = request.CustomerName,
                IssueDate = DateTime.UtcNow,
                Status = InvoiceStatus.Open,
                TotalAmount = 0
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return MapToResponse(invoice);
        }

        public async Task AddItemAsync(Guid invoiceId, AddInvoiceItemRequest request)
        {
            _logger.LogInformation(
                "Adding item to invoice {InvoiceId}",
                invoiceId);

            var invoice = await _context.Invoices
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == invoiceId) ?? throw new BusinessException("Invoice not found.");

            ValidateItemRequest(request, invoice);

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

            invoice.AddAmount(total);
            _context.Invoices.Update(invoice);

            await _context.SaveChangesAsync();
        }

        public async Task CloseAsync(Guid invoiceId)
        {

            var invoice = await _context.Invoices
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == invoiceId) ?? throw new BusinessException("Invoice not found.");

            if (invoice.Items.Count == 0)
                throw new BusinessException(
                    "Cannot close an invoice without items.");

            _logger.LogInformation("Closing invoice {InvoiceId}", invoiceId);

            if (invoice.Status == InvoiceStatus.Closed)
                throw new BusinessException("Invoice is already closed.");

            invoice.Close();

            await _context.SaveChangesAsync();
        }

        public async Task<InvoiceResponse?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation(
                "Fetching invoice {InvoiceId}",
                id);

            var invoice = await _context.Invoices
                .AsNoTracking()
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
                return null;

            return MapToResponse(invoice);
        }

        public async Task<List<InvoiceResponse>> GetAllAsync(
            string? customer,
            DateTime? startDate,
            DateTime? endDate,
            string? status)
        {

            _logger.LogInformation(
                "Fetching invoices with filters");

            startDate ??= DateTime.UtcNow.Date.AddDays(-30);

            var query = _context.Invoices
                .AsNoTracking()
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

            var invoices = await query.OrderByDescending(x => x.IssueDate).ToListAsync();

            return [.. invoices.Select(MapToResponse)];
        }

        private static InvoiceResponse MapToResponse(Invoice invoice)
        {
            return new InvoiceResponse
            {
                Id = invoice.Id,
                Number = invoice.Number,
                CustomerName = invoice.CustomerName,
                IssueDate = invoice.IssueDate,
                Status = invoice.Status.ToString(),
                TotalAmount = invoice.TotalAmount,
                Items = [.. invoice.Items.Select(x => new InvoiceItemResponse
                {
                    Id = x.Id,
                    Description = x.Description,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    TotalItemAmount = x.TotalItemAmount,
                    Justification = x.Justification
                })]
            };
        }

        private static void ValidateItemRequest(
            AddInvoiceItemRequest request,
            Invoice invoice)
        {
            if (invoice.Status == InvoiceStatus.Closed)
                throw new BusinessException("Closed invoice cannot be changed.");

            if (string.IsNullOrWhiteSpace(request.Description) ||
                request.Description.Length < 3)
                throw new BusinessException(
                    "Description must have at least 3 characters.");

            if (request.Quantity <= 0)
                throw new BusinessException(
                    "Quantity must be greater than zero.");

            if (request.UnitPrice <= 0)
                throw new BusinessException(
                    "Unit price must be greater than zero.");
        }
    }
}
