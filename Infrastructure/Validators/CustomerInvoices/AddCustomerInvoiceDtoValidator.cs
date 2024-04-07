using Core.Entities;
using FluentValidation;
using Infrastructure.DTOs.CustomerInvoices;
using Infrastructure.DTOs.Customers;

namespace Infrastructure.Validators.CustomerInvoices;

public class AddCustomerInvoiceDtoValidator : AbstractValidator<AddCustomerInvoiceDto>
{
    public AddCustomerInvoiceDtoValidator()
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
        
        RuleFor(a => a.CustomerId)
            .NotEmpty()
            .WithMessage("CustomerId can't be empty");
    }
}