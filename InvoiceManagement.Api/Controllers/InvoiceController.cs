using InvoiceManagement.Application.DTOs.Invoices;
using InvoiceManagement.Application.Interfaces;
using InvoiceManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManagement.Api.Controllers
{
    /// <summary>
    /// Controller responsible for invoice management operations,
    /// including creation, querying, item management and closing invoices.
    /// </summary>
    /// <param name="invoiceService">
    /// Service responsible for invoice business operations.
    /// </param>
    [ApiController]
    [Route("invoices")]
    public class InvoiceController(IInvoiceService invoiceService) : ControllerBase
    {
        private readonly IInvoiceService _invoiceService = invoiceService;

        /// <summary>
        /// Creates a new invoice with initial status Open.
        /// </summary>
        /// <param name="request">Invoice creation data.</param>
        /// <returns>The created invoice.</returns>
        /// <response code="201">Invoice created successfully.</response>
        /// <response code="400">Invalid request data.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request)
        {
            var invoice = await _invoiceService.CreateAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
        }

        /// <summary>
        /// Adds a new item to an existing invoice.
        /// </summary>
        /// <param name="id">Invoice identifier.</param>
        /// <param name="request">Item data to be added.</param>
        /// <returns>No content when successful.</returns>
        /// <response code="204">Item added successfully.</response>
        /// <response code="400">Validation or business rule error.</response>
        /// <response code="404">Invoice not found.</response>
        [HttpPost("{id}/items")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddItem(Guid id, [FromBody] AddInvoiceItemRequest request)
        {
            await _invoiceService.AddItemAsync(id, request);

            return NoContent();
        }

        /// <summary>
        /// Closes an existing invoice.
        /// After closing, no changes are allowed.
        /// </summary>
        /// <param name="id">Invoice identifier.</param>
        /// <returns>No content when successful.</returns>
        /// <response code="204">Invoice closed successfully.</response>
        /// <response code="400">Business rule validation error.</response>
        /// <response code="404">Invoice not found.</response>
        [HttpPut("{id}/close")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Close(Guid id)
        {
            await _invoiceService.CloseAsync(id);

            return NoContent();
        }

        /// <summary>
        /// Retrieves an invoice by its identifier.
        /// </summary>
        /// <param name="id">Invoice identifier.</param>
        /// <returns>The invoice data.</returns>
        /// <response code="200">Invoice found successfully.</response>
        /// <response code="404">Invoice not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);

            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }

        /// <summary>
        /// Retrieves all invoices with optional filters.
        /// </summary>
        /// <param name="customer">Customer name filter.</param>
        /// <param name="startDate">
        /// Initial issue date filter (example: 2026-04-10).
        /// </param>
        /// <param name="endDate">
        /// Final issue date filter (example: 2026-04-11).
        /// </param>
        /// <param name="status">Invoice status filter (Open or Closed).</param>
        /// <returns>List of filtered invoices.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? customer,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? status = null)
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
