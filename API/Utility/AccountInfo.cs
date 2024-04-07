namespace API.Utility;

public class AccountInfo
{
    public long Code { get; set; }
    public string EnglishName { get; set; } = null!;
    public string ArabicName { get; set; } = null!;
    public decimal Balance { get; set; }
    public int Level { get; set; }
    public bool IsMain { get; set; }
}