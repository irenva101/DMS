using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DMS.API.Helper
{
    public class RemoveVersionParameterForPublicEndpoints : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            var hasApiVersionAttribute = apiDescription.ActionDescriptor
                .EndpointMetadata.OfType<ApiVersionAttribute>().Any();

            if (!hasApiVersionAttribute)
            {
                var versionParam = operation.Parameters?.FirstOrDefault(p =>
                    p.Name == "api-version" && p.In == ParameterLocation.Query);

                if (versionParam != null)
                {
                    // Parameters can not be null because we already used it to get versionParam.
                    operation.Parameters!.Remove(versionParam);
                }
            }
        }
    }
}
