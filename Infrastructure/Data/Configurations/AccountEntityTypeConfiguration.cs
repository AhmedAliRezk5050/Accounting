using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class AccountEntityTypeConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.Property(p => p.Balance)
                .HasComputedColumnSql("[Debit]-[Credit]");

            builder.Property(p => p.Currency).HasMaxLength(100);
            builder.Property(p => p.EnglishName).HasMaxLength(100);
            builder.Property(p => p.ArabicName).HasMaxLength(100);
            
            builder.Property(entry => entry.Debit).HasPrecision(17, 2);
            builder.Property(entry => entry.Credit).HasPrecision(17, 2);
            builder.Property(entry => entry.Balance).HasPrecision(17, 2);

            builder.HasIndex(a => a.ArabicName).IsUnique();
            builder.HasIndex(a => a.EnglishName).IsUnique();
            builder.HasIndex(a => a.Code).IsUnique();
            
            builder.HasOne(x => x.Parent)
                .WithMany(x => x.SubAccounts)
                .HasForeignKey(x => x.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            
            
            
            builder.HasMany(a => a.EntryDetails)
                .WithOne(e => e.Account)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}