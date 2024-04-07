using Infrastructure.DTOs.EntryDetails;

namespace Infrastructure.DTOs.Entries;

public class AddEntryDto
{
  public string? Description { get; set; }
  
  public DateTime? EntryDate;

  public bool IsOpening { get; set; }
  
  public IList<AddEntryDetailsDto>? EntryDetails { get; set; }
}