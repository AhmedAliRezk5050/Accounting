using FluentValidation;
using Infrastructure.DTOs.Entries;
using Infrastructure.Validators.EntryDetails;

namespace Infrastructure.Validators.Entries;

public class AddEntryDtoValidator : AbstractValidator<AddEntryDto>
{
    public AddEntryDtoValidator()
    {
        RuleFor(a => a.Description)
            .NotEmpty()
            .WithMessage("Description can't be empty")
            .MinimumLength(3)
            .WithMessage("Description can't be less than 3 characters");

        RuleFor(a => a)
            .Must(e => e.IsOpening || e.EntryDate is not null)
            .WithMessage("Entry date can't be empty");

        RuleFor(a => a.EntryDetails)
            .NotEmpty()
            .WithMessage("Entry Details can't be empty")
            .Must(entryDetails => entryDetails is null || entryDetails.Count > 1)
            .WithMessage("Entry Details can't be less than 1");

        RuleForEach(a => a.EntryDetails)
            .SetValidator(new AddEntryDetailsValidators());
    }
}