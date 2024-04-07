using Core.Entities;

namespace Core.Interfaces;

public interface IEntryRepository : IRepository<Entry>
{
    void Post(Entry entry);
}