using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Application.DTOs.Validations
{
    public class PositiveValueAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is decimal val && val <= 0)
            {
                return new ValidationResult(ErrorMessage ?? "O valor deve ser maior que zero.");
            }

            return ValidationResult.Success;
        }
    }
}
