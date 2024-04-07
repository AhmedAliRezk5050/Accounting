using FluentValidation;
using Infrastructure.DTOs.Customers;

namespace Infrastructure.Validators.Customers;

public class AddCustomerDtoValidator : AbstractValidator<AddCustomerDto>
{
    public AddCustomerDtoValidator()
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
    }
}