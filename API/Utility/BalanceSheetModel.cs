namespace API.Utility;

public class BalanceSheetModel
{
    public decimal AssetsAmount { get; set; }
    public decimal LiabilitiesAmount { get; set; }
    public decimal EquityAmount { get; set; }
    
    public List<AccountInfo> AssetsAccountsInfoList { get; set; } = null!;
    public List<AccountInfo> LiabilitiesAccountsInfoList { get; set; } = null!;
    public List<AccountInfo> EquityAccountsInfoList { get; set; } = null!;
    
    public string FromDate { get; set; } = null!;
    public string ToDate { get; set; } = null!;

    public decimal NetIncome { get; set; }
}