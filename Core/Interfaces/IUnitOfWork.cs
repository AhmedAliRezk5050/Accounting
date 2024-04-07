namespace Core.Interfaces
{
    public interface IUnitOfWork
    {
        public IAccountRepository AccountRepository { get; set; }
        public IEntryRepository EntryRepository { get; set; }
        public IEntryDetailsRepository EntryDetailsRepository { get; set; }
        public ICustomerRepository CustomerRepository { get; set; }
        public ICustomerInvoiceRepository CustomerInvoiceRepository { get; set; }
        public ISupplierRepository SupplierRepository { get; set; }
        public ISupplierInvoiceRepository SupplierInvoiceRepository { get; set; }
        Task<bool> SaveAsync();
    }
}
