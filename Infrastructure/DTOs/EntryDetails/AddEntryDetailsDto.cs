using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DTOs.EntryDetails;

public class AddEntryDetailsDto
{
    public decimal? Debit { get; set; }
    
    public decimal? Credit { get; set; }

    public string? Description { get; set; }
    
    public int? AccountId { get; set; }
}