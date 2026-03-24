using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Application.DTOs.Validations
{
    public class PositiveValueAttribute : ValidationAttribute
    {
        // O ASP.NET Core executa automaticamente este método durante a validação do modelo ao encontrar a anotação correspondente em uma propriedade.
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
