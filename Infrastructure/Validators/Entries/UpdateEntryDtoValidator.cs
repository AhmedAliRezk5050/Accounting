using FluentValidation;
using Infrastructure.DTOs.Entries;
using Infrastructure.Validators.EntryDetails;

namespace Infrastructure.Validators.Entries;

public class UpdateEntryDtoValidator : AbstractValidator<UpdateEntryDto>
{
    public UpdateEntryDtoValidator()
    {
        RuleFor(a => a.Description)
            .NotEmpty()
            .WithMessage("Description can't be empty")
            .MinimumLength(3)
            .WithMessage("Description can't be less than 3 characters");

        RuleFor(a => a.EntryDate)
            .NotEmpty()
            .WithMessage("Entry date can't be empty");
        
        RuleFor(a => a.IsOpening)
            .NotEmpty()
            .WithMessage("Entry type can't be empty");

        RuleFor(a => a.EntryDetails)
            .NotEmpty()
            .WithMessage("Entry Details can't be empty")
            .Must(entryDetails => entryDetails is null || entryDetails.Count > 1)
            .WithMessage("Entry Details can't be less than 1");

        RuleForEach(e => e.EntryDetails)
            .SetValidator(new UpdateEntryDetailsValidators());
    }
}