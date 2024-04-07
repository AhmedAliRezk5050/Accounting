using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repository;

public class CustomerInvoiceRepository : BaseRepository<CustomerInvoice>, ICustomerInvoiceRepository

{
    public CustomerInvoiceRepository(AppDbContext context) : base(context)
    {
            
    }
}
