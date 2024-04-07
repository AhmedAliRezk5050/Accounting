using Infrastructure.DTOs.EntryDetails;

namespace Infrastructure.DTOs.Entries;

public class UpdateEntryDto
{
    public string? Description { get; set; }
  
    public DateTime? EntryDate;

    public bool? IsOpening { get; set; }
    
    public IList<UpdateEntryDetailsDto>? EntryDetails { get; set; }
}