namespace API.Utility;

public class GeneralLedgerQueryResponse
{
    public static decimal BalanceAccumulator;

    public DateTime Date { get; set; }
    public string Description { get; set; } = null!;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }

    private decimal _balance;

    public decimal Balance
    {
        get => _balance;
        set
        {
            BalanceAccumulator = BalanceAccumulator + Debit - Credit;
            _balance = BalanceAccumulator;
        }
    }

    public int EntryId { get; set; }
}