using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            
            builder.HasIndex(a => a.ArabicName).IsUnique();
            builder.HasIndex(a => a.EnglishName).IsUnique();
            builder.HasIndex(a => a.PhoneNumber).IsUnique();
            builder.HasIndex(a => a.TaxNumber).IsUnique();
            
            builder.HasMany(x => x.CustomerInvoices)
                .WithOne(x => x.Customer)
                .HasForeignKey(x => x.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Account)
                .WithOne(x => x.Customer)
                .HasForeignKey<Customer>(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}