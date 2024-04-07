using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repository;

public class EntryDetailsRepository : BaseRepository<EntryDetails>, IEntryDetailsRepository
{
    public EntryDetailsRepository(AppDbContext context) : base(context)
    {
    }
}