namespace Infrastructure.DTOs.Customers;

public class AddCustomerDto
{
    public string ArabicName { get; set; } = null!;
    
    public string EnglishName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string BankAccountNumber { get; set; } = null!;

    public string BankName { get; set; } = null!;
    
    public string TaxNumber { get; set; } = null!;
}