using FluentValidation;
using Infrastructure.DTOs.Users;

namespace Infrastructure.Validators.Users;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(a => a.UserName)
            .NotEmpty()
            .WithMessage("UserName can't be empty");
        
        RuleFor(a => a.Password)
            .NotEmpty()
            .WithMessage("UserName can't be empty")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.")
            .Must(ValidatePasswordComplexity)
            .WithMessage("Password must meet complexity requirements.");
    }
    
    private static bool ValidatePasswordComplexity(string password)
    {
        if (!password.Any(char.IsLower))
        {
            return false;
        }

        if (!password.Any(char.IsUpper))
        {
            return false;
        }

        if (!password.Any(char.IsDigit))
        {
            return false;
        }

        if (password.All(char.IsLetterOrDigit))
        {
            return false;
        }

        return true;
    }
}