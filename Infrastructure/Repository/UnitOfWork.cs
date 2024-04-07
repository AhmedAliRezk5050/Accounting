using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public IAccountRepository AccountRepository { get; set; }
        public IEntryRepository EntryRepository { get; set; }
        public IEntryDetailsRepository EntryDetailsRepository { get; set; }
        public ICustomerRepository CustomerRepository { get; set; }
        public ICustomerInvoiceRepository CustomerInvoiceRepository { get; set; }
        public ISupplierRepository SupplierRepository { get; set; }
        public ISupplierInvoiceRepository SupplierInvoiceRepository { get; set; }
        

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            AccountRepository = new AccountRepository(dbContext);
            EntryRepository = new EntryRepository(dbContext);
            EntryDetailsRepository = new EntryDetailsRepository(dbContext);
            CustomerRepository = new CustomerRepository(dbContext);
            CustomerInvoiceRepository = new CustomerInvoiceRepository(dbContext);
            SupplierRepository = new SupplierRepository(dbContext);
            SupplierInvoiceRepository = new SupplierInvoiceRepository(dbContext);
        }

        public async Task<bool> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
