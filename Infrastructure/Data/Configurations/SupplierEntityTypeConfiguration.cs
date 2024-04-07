using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class SupplierEntityTypeConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            
            builder.HasIndex(a => a.ArabicName).IsUnique();
            builder.HasIndex(a => a.EnglishName).IsUnique();
            builder.HasIndex(a => a.PhoneNumber).IsUnique();
            builder.HasIndex(a => a.TaxNumber).IsUnique();
            
            builder.HasMany(x => x.SupplierInvoices)
                .WithOne(x => x.Supplier)
                .HasForeignKey(x => x.SupplierId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Account)
                .WithOne(x => x.Supplier)
                .HasForeignKey<Supplier>(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}