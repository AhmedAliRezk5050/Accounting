namespace API.Utility;

public class GetGeneralLedgerByAccountCodeResponse
{
    public List<GeneralLedgerQueryResponse> Data { get; set; } = null!;
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public  decimal TotalBalance { get; set; }
    public string FromDate { get; set; } = null!;
    public string ToDate { get; set; } = null!;
    public long AccountCode { get; set; }
    public string AccountArabicName { get; set; } = null!;
    public string AccountEnglishName { get; set; } = null!;

    public int AccountId { get; set; }
}