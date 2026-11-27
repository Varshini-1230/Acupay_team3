
using System.Collections.Generic;
using System.Threading.Tasks;
using AcuPay.Models;

namespace AcuPay.Repositories
{
    public interface IInvoiceRepository
    {
        // Create
        Task<int> AddInvoiceAsync(Invoice invoice);

        // Read
        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);

        Task<List<Invoice>> GetAllInvoicesAsync();

        // Update
        Task UpdateInvoiceAsync(Invoice invoice);

        // Delete
        Task DeleteInvoiceAsync(int invoiceId);
    }
}
