using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repository;

public class SupplierInvoiceRepository : BaseRepository<SupplierInvoice>, ISupplierInvoiceRepository

{
    public SupplierInvoiceRepository(AppDbContext context) : base(context)
    {
            
    }
}
