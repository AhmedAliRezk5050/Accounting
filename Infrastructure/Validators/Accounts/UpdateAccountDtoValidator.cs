using FluentValidation;
using Infrastructure.DTOs.Accounts;

namespace Infrastructure.Validators.Accounts;
 
public class UpdateAccountDtoValidator: AbstractValidator<UpdateAccountDto>
{
    public UpdateAccountDtoValidator()
    {
        
        RuleFor(a => a.EnglishName)
            .NotEmpty()
            .WithMessage("Account english name can't be empty");
        
        RuleFor(a => a.ArabicName)
            .NotEmpty()
            .WithMessage("Account arabic  can't be empty");
        
        RuleFor(a => a.Currency)
            .NotEmpty()
            .WithMessage("Currency  can't be empty");

    }
}