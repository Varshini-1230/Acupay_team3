
using Microsoft.AspNetCore.Mvc;
using AcuPay.Services;
using AcuPay.Repositories;
using AcuPay.Models;
using AcuPay.Enums;

namespace AcuPay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceMatchingService _matchingService;
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceController(
            InvoiceMatchingService matchingService,
            IInvoiceRepository invoiceRepository)
        {
            _matchingService = matchingService;
            _invoiceRepository = invoiceRepository;
        }

        // GET: api/Invoice
        // Get all invoices
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices =
                await _invoiceRepository.GetAllInvoicesAsync();

            return Ok(invoices);
        }

        // GET: api/Invoice/1
        // Get invoice by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var invoice =
                await _invoiceRepository.GetInvoiceByIdAsync(id);

            if (invoice == null)
            {
                return NotFound("Invoice not found.");
            }

            return Ok(invoice);
        }

        // POST: api/Invoice
        // Create a new invoice
        [HttpPost]
        public async Task<IActionResult> CreateInvoice(
            [FromBody] Invoice invoice)
        {
            if (invoice == null)
            {
                return BadRequest("Invoice details are required.");
            }

            if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
            {
                return BadRequest("Invoice number is required.");
            }

            if (invoice.InvoiceAmount <= 0)
            {
                return BadRequest("Invoice amount must be greater than zero.");
            }

            if (invoice.VendorId <= 0)
            {
                return BadRequest("Valid Vendor ID is required.");
            }

            if (string.IsNullOrWhiteSpace(invoice.CreatedBy))
            {
                return BadRequest("CreatedBy is required.");
            }

            try
            {
                invoice.Status = InvoiceStatus.Pending;
                invoice.CreatedAtUtc = DateTime.UtcNow;

                int invoiceId =
                    await _invoiceRepository.AddInvoiceAsync(invoice);

                invoice.InvoiceId = invoiceId;

                return CreatedAtAction(
                    nameof(GetInvoiceById),
                    new { id = invoiceId },
                    invoice
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Invoice/1
        // Update an existing invoice
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(
            int id,
            [FromBody] Invoice invoice)
        {
            if (invoice == null)
            {
                return BadRequest("Invoice details are required.");
            }

            if (id <= 0)
            {
                return BadRequest("Invalid Invoice ID.");
            }

            if (invoice.InvoiceAmount <= 0)
            {
                return BadRequest("Invoice amount must be greater than zero.");
            }

            var existingInvoice =
                await _invoiceRepository.GetInvoiceByIdAsync(id);

            if (existingInvoice == null)
            {
                return NotFound("Invoice not found.");
            }

            // Update editable fields
            existingInvoice.InvoiceNumber = invoice.InvoiceNumber;
            existingInvoice.VendorId = invoice.VendorId;
            existingInvoice.PoId = invoice.PoId;
            existingInvoice.InvoiceAmount = invoice.InvoiceAmount;
            existingInvoice.InvoiceDate = invoice.InvoiceDate;

            await _invoiceRepository.UpdateInvoiceAsync(existingInvoice);

            return Ok(existingInvoice);
        }

        // DELETE: api/Invoice/1
        // Delete an invoice
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Invoice ID.");
            }

            var invoice =
                await _invoiceRepository.GetInvoiceByIdAsync(id);

            if (invoice == null)
            {
                return NotFound("Invoice not found.");
            }

            await _invoiceRepository.DeleteInvoiceAsync(id);

            return Ok("Invoice deleted successfully.");
        }

        // POST: api/Invoice/1/match
        // Match invoice against its purchase order
        [HttpPost("{id}/match")]
        public async Task<IActionResult> MatchInvoice(int id)
        {
            try
            {
                var result =
                    await _matchingService.MatchInvoiceAsync(id);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
