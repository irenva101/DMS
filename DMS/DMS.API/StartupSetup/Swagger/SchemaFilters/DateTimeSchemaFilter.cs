using DMS.Shared.Helper;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DMS.API.StartupSetup.Swagger.SchemaFilters
{
    public class DateTimeSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(DateTime) || context.Type == typeof(DateTime?))
            {
                schema.Format = "date-time";
                schema.Example = new OpenApiString(DateTime.UtcNow.ToString(DateTimeHelper.Iso8601DateTimeFormat));
            }
        }
    }
}
