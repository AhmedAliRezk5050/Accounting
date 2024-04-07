namespace API.Utility;

public class GetTrialBalanceResponse
{
  public List<TrialBalanceQueryResponse> TrialBalanceQueryResponseList { get; set; } = null!;
  public decimal TotalOpeningDebit { get; set; }
  public decimal TotalOpeningCredit { get; set; }
  public decimal TotalDebit { get; set; }
  public decimal TotalCredit { get; set; }
  
  public decimal TotalEndingDebit { get; set; }
  public decimal TotalEndingCredit { get; set; }
  
  public string FromDate { get; set; } = null!;
  public string ToDate { get; set; }  = null!;
}