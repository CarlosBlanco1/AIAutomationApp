using System.ComponentModel.DataAnnotations;

public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly int _maxFileSize = 1 * 1024 * 1024;
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value is IFormFile file)
        {
            if (file.Length > _maxFileSize)
            {
                return new ValidationResult(ErrorMessage ?? $"The file is too large. Maximum size is {_maxFileSize / (1024 * 1024)} MB.");
            }
            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult(ErrorMessage ?? "File not in the right format");
        }
    }
}