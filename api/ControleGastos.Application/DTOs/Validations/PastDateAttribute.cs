using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Application.DTOs.Validations
{
    public class PastDateAttribute : ValidationAttribute
    {
        // O ASP.NET Core executa automaticamente este método durante a validação do modelo ao encontrar a anotação correspondente em uma propriedade.
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
