using DMS.Shared.Config;
using System.ComponentModel.DataAnnotations;

namespace DMS.Shared.Helper
{
    public static class DmsConfigValidator
    {
        public static void Validate(DmsConfig appConfig)
        {
            var validationContext = new ValidationContext(appConfig);

            try
            {
                Validator.ValidateObject(appConfig, validationContext, true);
            }
            catch (ValidationException ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}
