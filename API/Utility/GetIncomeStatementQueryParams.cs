using System.ComponentModel.DataAnnotations;

namespace API.Utility;

public class GetIncomeStatementQueryParams
{
  [Required]
  public DateTime? From { get; set; }
  public DateTime? To { get; set; } = DateTime.UtcNow;
}