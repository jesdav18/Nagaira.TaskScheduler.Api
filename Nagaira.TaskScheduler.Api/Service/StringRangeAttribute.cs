using System.ComponentModel.DataAnnotations;

namespace Nagaira.TaskScheduler.Api.Service
{
    public class StringRangeAttribute : ValidationAttribute
    {
        public string[]? AllowableValues { get; set; }

        protected ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string? sValue = value as string;
            if (AllowableValues?.Contains(sValue?.ToUpper()?.ToString()) == true)
            {
                return ValidationResult.Success;
            }
            var msg = $"La petición http enviada no es correcta solo permite: {string.Join(", ", AllowableValues ?? new string[] { "No allowable values found" })}.";
            return new ValidationResult(msg);
        }
    }
}
