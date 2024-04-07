using Core.Entities;
using FluentValidation;
using Infrastructure.DTOs.CustomerInvoices;
using Infrastructure.DTOs.Customers;
using Infrastructure.DTOs.SupplierInvoices;

namespace Infrastructure.Validators.SupplierInvoices;

public class AddSupplierInvoiceDtoValidator : AbstractValidator<AddSupplierInvoiceDto>
{
    public AddSupplierInvoiceDtoValidator()
    {
        RuleFor(a => a.InvoiceNumber)
            .NotEmpty()
            .WithMessage("InvoiceNumber can't be empty");
        
        RuleFor(a => a.Date)
            .NotEmpty()
            .WithMessage("Date can't be empty");
        
        RuleFor(a => a.Amount)
            .NotEmpty()
            .WithMessage("Amount can't be empty");
        
        RuleFor(a => a.Tax)
            .NotEmpty()
            .WithMessage("Tax can't be empty");
        
        RuleFor(a => a.TotalAmount)
            .NotEmpty()
            .WithMessage("TotalAmount can't be empty");
        
        RuleFor(a => a.SupplierId)
            .NotEmpty()
            .WithMessage("CustomerId can't be empty");
    }
}