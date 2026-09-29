using DMS.Shared.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DMS.API.StartupSetup.Swagger
{
    public class SwaggerHiddenSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context.Type == null)
                return;

            var hiddenProperties = context.Type
                .GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(SwaggerHiddenAttribute), false).Any())
                .Select(p => ToCamelCase(p.Name));

            foreach (var propertyName in hiddenProperties)
            {
                schema.Properties.Remove(propertyName);
            }
        }

        private static string ToCamelCase(string name) =>
            string.IsNullOrEmpty(name)
                ? name
                : char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
}
