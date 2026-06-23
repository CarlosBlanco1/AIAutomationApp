using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

class PasswordAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value is not string password)
        {
            return new ValidationResult("Password is mandatory.");
        }
        if(!Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
        {
            return new ValidationResult("Password must contain a symbol.");
        }
        if(!(Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(value as string, "[a-z]")))
        {
            return new ValidationResult("Password must contain upper and lower case.");
        }
        if(!Regex.IsMatch(password, "[0-9]"))
        {
            return new ValidationResult("Password must contain a number");
        }
        return ValidationResult.Success;
    }
}