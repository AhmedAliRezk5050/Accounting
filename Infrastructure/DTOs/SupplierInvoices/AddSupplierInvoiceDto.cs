﻿namespace Infrastructure.DTOs.SupplierInvoices;

public class AddSupplierInvoiceDto
{
    public string InvoiceNumber { get; set; } = null!;

    public DateTime? Date { get; set; }
    
    public decimal? Amount { get; set; }
    
    public decimal? Tax { get; set; }
    
    public decimal? TotalAmount { get; set; }

    public string? Notes { get; set; }
    
    public int? SupplierId { get; set; }
}