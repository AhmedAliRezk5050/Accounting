using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class SupplierInvoiceEntityTypeConfiguration : IEntityTypeConfiguration<SupplierInvoice>
    {
        public void Configure(EntityTypeBuilder<SupplierInvoice> builder)
        {
            
            builder.HasIndex(a => a.InvoiceNumber).IsUnique();
            
            builder.Property(entry => entry.Amount).HasPrecision(17, 2);
            builder.Property(entry => entry.Tax).HasPrecision(17, 2);
            builder.Property(entry => entry.TotalAmount).HasPrecision(17, 2);
        }
    }
}