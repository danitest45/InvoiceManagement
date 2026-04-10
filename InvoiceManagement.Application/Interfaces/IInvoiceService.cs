using InvoiceManagement.Application.DTOs.Invoices;

namespace InvoiceManagement.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task AddItemAsync(Guid invoiceId, AddInvoiceItemRequest request);
        Task CloseAsync(Guid invoiceId);
        Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request);
        Task<List<InvoiceResponse>> GetAllAsync(string? customer, DateTime? startDate, DateTime? endDate, string? status);
        Task<InvoiceResponse?> GetByIdAsync(Guid id);
    }
}