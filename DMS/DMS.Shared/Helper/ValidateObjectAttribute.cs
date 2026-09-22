using DMS.Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace DMS.Shared.Helper
{
    public class ValidateObjectAttribute : ValidationAttribute
    {
        private readonly AppSettingsEnvironment _validationEnvironment;
        private readonly string _environment;

        public ValidateObjectAttribute(AppSettingsEnvironment validationEnvironment)
        {
            _validationEnvironment = validationEnvironment;
            _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }

        public ValidateObjectAttribute()
        {
            _validationEnvironment = AppSettingsEnvironment.All;
            _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }

        protected override ValidationResult IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (
                _validationEnvironment == AppSettingsEnvironment.All
                || _environment == _validationEnvironment.ToString()
            )
            {
                if (value == null)
                {
                    return new ValidationResult(
                        $"Validation failed for {validationContext.DisplayName}"
                    );
                }

                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(value, null, null);
                bool isValid = Validator.TryValidateObject(
                    value,
                    context,
                    validationResults,
                    validateAllProperties: true
                );

                if (!isValid)
                {
                    var errorMessages = string.Join(
                        ", ",
                        validationResults.Select(result => result.ErrorMessage)
                    );
                    return new ValidationResult(
                        $"Validation failed for {validationContext.DisplayName}: {errorMessages}"
                    );
                }
            }
            return ValidationResult.Success;
        }
    }
}
