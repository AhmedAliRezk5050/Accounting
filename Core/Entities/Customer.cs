namespace Core.Entities;

public class Customer
{
    public int Id { get; set; }
    public string ArabicName { get; set; } = null!;
    public string EnglishName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string BankAccountNumber { get; set; } = null!;

    public string BankName { get; set; } = null!;
    
    public string TaxNumber { get; set; } = null!;
    
    public IList<CustomerInvoice> CustomerInvoices { get; set; } = null!;
    
    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;
}