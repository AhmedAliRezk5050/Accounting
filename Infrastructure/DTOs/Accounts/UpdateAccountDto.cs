namespace Infrastructure.DTOs.Accounts;

public class UpdateAccountDto
{
    public string EnglishName { get; set; } = null!;
    public string ArabicName { get; set; } = null!;
    public string Currency { get; set; } = null!;
}