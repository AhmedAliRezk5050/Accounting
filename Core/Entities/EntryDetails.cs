namespace Core.Entities;

public class EntryDetails
{
    public int Id { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }

    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;
    
    public int EntryId { get; set; }
    public Entry Entry { get; set; } = null!;
}