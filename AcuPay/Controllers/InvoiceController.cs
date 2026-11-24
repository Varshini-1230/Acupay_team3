
using Microsoft.AspNetCore.Mvc;
using AcuPay.Services;
using AcuPay.Repositories;

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
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices =
                await _invoiceRepository.GetAllInvoicesAsync();

            return Ok(invoices);
        }

        // GET: api/Invoice/101
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

        // POST: api/Invoice/101/match
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
