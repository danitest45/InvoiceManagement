using InvoiceManagement.Application.DTOs.Invoices;
using InvoiceManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManagement.Api.Controllers
{
    [ApiController]
    [Route("invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;

        public InvoicesController(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request)
        {
            var invoice = await _invoiceService.CreateAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
        }

        [HttpPost("{id}/items")]
        public async Task<IActionResult> AddItem(Guid id, [FromBody] AddInvoiceItemRequest request)
        {
            await _invoiceService.AddItemAsync(id, request);

            return NoContent();
        }

        [HttpPut("{id}/close")]
        public async Task<IActionResult> Close(Guid id)
        {
            await _invoiceService.CloseAsync(id);

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);

            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? customer,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? status)
        {
            var invoices = await _invoiceService.GetAllAsync(
                customer,
                startDate,
                endDate,
                status);

            return Ok(invoices);
        }
    }
}
