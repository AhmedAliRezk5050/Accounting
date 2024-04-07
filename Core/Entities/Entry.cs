
namespace Core.Entities;

public class Entry
{
    public int Id { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }

    public string Description { get; set; } = null!;
    
    public bool IsPosted { get; set; }
    
    public bool IsOpening { get; set; }

    public DateTime EntryDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public IList<EntryDetails> EntryDetails { get; set; } = null!;
    
}