using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class EntryDetailsEntityTypeConfiguration : IEntityTypeConfiguration<EntryDetails>
{
    public void Configure(EntityTypeBuilder<EntryDetails> builder)
    {
        // roles
        builder.Property(entry => entry.Debit).HasPrecision(17, 2);
        builder.Property(entry => entry.Credit).HasPrecision(17, 2);
    }
}