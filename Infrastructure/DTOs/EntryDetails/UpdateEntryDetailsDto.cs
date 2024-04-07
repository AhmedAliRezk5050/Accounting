namespace Infrastructure.DTOs.EntryDetails;

public class UpdateEntryDetailsDto
{
    public int Id { get; set; }
    public decimal? Debit { get; set; }
    
    public decimal? Credit { get; set; }

    public string? Description { get; set; }
    
    public int? AccountId { get; set; }
}