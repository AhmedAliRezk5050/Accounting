namespace API.Utility;

public class TrialBalanceQueryResponse
{
  public long AccountCode { get; set; }
  public string AccountArabicName { get; set; } = null!;
  public string AccountEnglishName { get; set; } = null!;
  
  public decimal OpeningDebit { get; set; }
  public decimal OpeningCredit { get; set; }
  
  public decimal Debit { get; set; }
  public decimal Credit { get; set; }

  public decimal EndingDebit => (OpeningDebit + Debit) - (OpeningCredit + Credit) > 0 ? (OpeningDebit + Debit) - (OpeningCredit + Credit) : 0;
  public decimal EndingCredit => (OpeningDebit + Debit) - (OpeningCredit + Credit) < 0 ? Math.Abs((OpeningDebit + Debit) - (OpeningCredit + Credit)) : 0;
}