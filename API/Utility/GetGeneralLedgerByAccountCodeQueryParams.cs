using System.ComponentModel.DataAnnotations;

namespace API.Utility;

public class GetGeneralLedgerByAccountCodeQueryParams
{
  [Required]
  public DateTime? FromDate { get; set; }
  public DateTime? ToDate { get; set; } = DateTime.UtcNow;
}