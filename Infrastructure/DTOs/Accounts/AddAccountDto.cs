
namespace Infrastructure.DTOs.Accounts;

public class AddAccountDto
{
  public string EnglishName { get; set; } = null!;
  public string ArabicName { get; set; } = null!;

  public string? Currency { get; set; }

  public int? ParentId { get; set; }
}