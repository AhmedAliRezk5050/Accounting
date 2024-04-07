using FluentValidation;
using Infrastructure.DTOs.Accounts;

namespace Infrastructure.Validators.Accounts;

public class AddAccountDtoValidator : AbstractValidator<AddAccountDto>
{
    public AddAccountDtoValidator()
    {
        RuleFor(a => a.EnglishName)
            .NotEmpty()
            .WithMessage("Account english name can't be empty");

        RuleFor(a => a.ArabicName)
            .NotEmpty()
            .WithMessage("Account arabic  can't be empty");

        RuleFor(a => a.Currency)
            .MinimumLength(5)
            .WithMessage("Account currency can't be less than 5 characters");
    }
}