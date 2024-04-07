using FluentValidation;
using Infrastructure.DTOs.EntryDetails;

namespace Infrastructure.Validators.EntryDetails;

public class AddEntryDetailsValidators  : AbstractValidator<AddEntryDetailsDto>
{
    public AddEntryDetailsValidators()
    {
        RuleFor(entry => entry.Debit)
            .NotNull()
            .GreaterThan(-1)
            .PrecisionScale(19, 4, false);
        
        RuleFor(entry => entry.Credit)
            .NotNull()
            .GreaterThan(-1)
            .PrecisionScale(19, 4, false);
        
        RuleFor(a => a.AccountId)
            .NotEmpty()
            .WithMessage("Account id can't be empty");
    }
}