using FluentValidation;
using Infrastructure.DTOs.Customers;
using Infrastructure.DTOs.Suppliers;

namespace Infrastructure.Validators.Suppliers;

public class AddSupplierDtoValidator : AbstractValidator<AddSupplierDto>
{
    public AddSupplierDtoValidator()
    {
        RuleFor(a => a.ArabicName)
            .NotEmpty()
            .WithMessage("ArabicName can't be empty");
        
        RuleFor(a => a.EnglishName)
            .NotEmpty()
            .WithMessage("EnglishName can't be empty");
        
        RuleFor(a => a.PhoneNumber)
            .NotEmpty()
            .WithMessage("PhoneNumber can't be empty");
        
        RuleFor(a => a.BankAccountNumber)
            .NotEmpty()
            .WithMessage("BankAccountNumber can't be empty");
        
        RuleFor(a => a.BankName)
            .NotEmpty()
            .WithMessage("BankName can't be empty");
        
        RuleFor(a => a.TaxNumber)
            .NotEmpty()
            .WithMessage("TaxNumber can't be empty");
        
        RuleFor(a => a.SupplierType)
            .NotEmpty()
            .WithMessage("SupplierType can't be empty");
    }
}