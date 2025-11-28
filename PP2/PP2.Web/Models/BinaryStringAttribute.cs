using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PP2.Web.Models
{
    /// <summary>
    /// Valida que un string:
    ///  - Solo contenga '0' y '1'
    ///  - Longitud > 0
    ///  - Longitud <= 8
    ///  - Longitud múltiplo de 2 (2, 4, 6 u 8)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class BinaryStringAttribute : ValidationAttribute
    {
        public BinaryStringAttribute()
        {
            // Mensaje por defecto; puedes personalizar por propiedad si quieres.
            ErrorMessage = "Debe ser una cadena binaria válida (solo 0 y 1), longitud 2, 4, 6 u 8.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var text = (value as string)?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(text))
                return new ValidationResult("El valor no puede estar vacío.");

            if (text.Length > 8)
                return new ValidationResult("La longitud no puede exceder 8 caracteres.");

            if (text.Length % 2 != 0)
                return new ValidationResult("La longitud debe ser múltiplo de 2 (2, 4, 6 u 8).");

            if (!text.All(c => c == '0' || c == '1'))
                return new ValidationResult("Solo se permiten los caracteres 0 y 1.");

            return ValidationResult.Success;
        }
    }
}