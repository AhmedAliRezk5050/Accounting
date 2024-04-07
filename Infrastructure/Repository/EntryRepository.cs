using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class EntryRepository : BaseRepository<Entry>, IEntryRepository
{
    public EntryRepository(AppDbContext context) : base(context)
    {
    }

    public IQueryable<Entry> Entries => _dbSet.AsNoTracking();
   

    public void Post(Entry entry)
    {
        entry.IsPosted = true;
        entry.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entry);
    }
}