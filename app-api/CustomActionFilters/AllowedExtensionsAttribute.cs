using System.ComponentModel.DataAnnotations;

public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] allowedExtensions = { ".pdf", ".docx", ".txt" };
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value is IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if(!allowedExtensions.Contains(fileExtension))
            {
                return new ValidationResult(ErrorMessage ?? "File not in the right format");
            }
            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult(ErrorMessage ?? "File not in the right format");
        }
    }
}