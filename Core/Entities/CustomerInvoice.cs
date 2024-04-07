namespace Core.Entities;

public class CustomerInvoice
{
    public int Id { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public DateTime Date { get; set; }
    
    public decimal Amount { get; set; }
    
    public decimal Tax { get; set; }
    
    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }
    
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}