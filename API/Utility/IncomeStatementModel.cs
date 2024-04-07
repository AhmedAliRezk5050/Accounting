namespace API.Utility;

public class IncomeStatementResponseModel
{
    public string FromDate { get; set; } = null!;
    public string ToDate { get; set; } = null!;
 
    public decimal RevenuesAmount { get; set; }
    public decimal RevenueReturnsAmount { get; set; }
    public decimal OperatingExpensesAmount { get; set; }
    public decimal AdministrativeExpensesAmount { get; set; }
    public decimal GeneralExpensesAmount { get; set; }
    public decimal SellingExpensesAmount { get; set; }
    public decimal FinancingExpensesAmount { get; set; }
    public decimal CapitalRevenuesAmount { get; set; }
    public decimal AccidentCompensationRevenuesAmount { get; set; }
    
    public decimal NetIncome { get; set; }
  
}