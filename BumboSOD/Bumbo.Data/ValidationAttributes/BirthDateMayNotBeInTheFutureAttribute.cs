using System.ComponentModel.DataAnnotations;

namespace Bumbo.Data.ValidationAttributes;

public class BirthDateMayNotBeInTheFutureAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTime birthDate)
        {
            return new ValidationResult("De geboortedatum is geen geldige datum.");
        }

        if (birthDate > DateTime.Now)
        {
            return new ValidationResult("De geboortedatum mag niet in de toekomst liggen.");
        }

        return ValidationResult.Success;
    }
}