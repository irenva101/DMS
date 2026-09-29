using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DMS.API.StartupSetup
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();

                // Get Enum names as strings
                var enumValues = Enum.GetValues(context.Type)
                    .Cast<Enum>()
                    .Select(e => new OpenApiString(e.ToString()));

                foreach (var value in enumValues)
                {
                    schema.Enum.Add(value);
                }

                schema.Type = "string";
            }
        }
    }

    public static class SwaggerConfig
    {
        public static void AddSwaggerUiConfiguration(this IApplicationBuilder app)
        {
            using var score = app.ApplicationServices.CreateScope();
            var provider = score.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/swagger"))
                {
                    context.Response.Headers.Append("X-Frame-Options", "DENY");
                    // Swagger UI's bundled JS (ajv schema validation) uses new Function()/eval
                    // internally, hence 'unsafe-eval'; confirmed no other violations via Report-Only testing.
                    context.Response.Headers.Append(
                        "Content-Security-Policy",
                        "default-src 'self'; script-src 'self' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; connect-src 'self'; frame-ancestors 'none'");
                }
                await next();
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/public/swagger.json", "Public API");
                c.SwaggerEndpoint("/swagger/desktop/swagger.json", "Desktop API");

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    if (description.GroupName == "external")
                        c.SwaggerEndpoint($"/swagger/external-{description.ApiVersion}/swagger.json",
                            $"External API {description.ApiVersion}");
                }
            });
        }
    }
}
