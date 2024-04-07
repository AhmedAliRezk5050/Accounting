using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class EntryEntityTypeConfiguration : IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        // relations
        builder.HasMany(entry => entry.EntryDetails)
            .WithOne(entryDetails => entryDetails.Entry)
            .OnDelete(DeleteBehavior.Cascade);

        
        // roles
        builder.Property(entry => entry.TotalDebit).HasPrecision(17, 2);
        builder.Property(entry => entry.TotalCredit).HasPrecision(17, 2);
    }
}