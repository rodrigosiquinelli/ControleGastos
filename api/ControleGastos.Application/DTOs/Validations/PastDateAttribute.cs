using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Application.DTOs.Validations
{
    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime date && date > DateTime.Today)
            {
                return new ValidationResult(ErrorMessage ?? "A data não pode ser no futuro.");
            }

            return ValidationResult.Success;
        }
    }
}
