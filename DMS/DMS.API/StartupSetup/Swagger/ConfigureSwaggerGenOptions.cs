using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using DMS.API.Helper;
using DMS.API.StartupSetup.Swagger.SchemaFilters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace DMS.API.StartupSetup.Swagger
{
    public class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly ILogger<ConfigureSwaggerGenOptions> _logger;

        private static readonly string[] _referencedAssemblies = new[]
        {
            "DMS.API"
        };

        public ConfigureSwaggerGenOptions(
            IApiVersionDescriptionProvider provider,
            ILogger<ConfigureSwaggerGenOptions> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public void Configure(SwaggerGenOptions c)
        {
            c.NonNullableReferenceTypesAsRequired();
            c.SwaggerDoc("public", new OpenApiInfo { Title = "Public API", Version = "v1" });
            c.SwaggerDoc("desktop", new OpenApiInfo { Title = "Desktop API", Version = "v1" });

            foreach (var description in _provider.ApiVersionDescriptions)
            {
                if (description.GroupName == "external")
                {
                    c.SwaggerDoc($"external-{description.ApiVersion}", new OpenApiInfo
                    {
                        Title = $"External API {description.ApiVersion}",
                        Version = description.ApiVersion.ToString()
                    });
                }
            }
            c.OperationFilter<RemoveVersionParameterForPublicEndpoints>();

            c.DocInclusionPredicate((docName, apiDesc) =>
            {
                var actionGroup = apiDesc.GroupName ?? "";

                if (docName == "public")
                {
                    return actionGroup == "public"
                        && !apiDesc.ActionDescriptor.EndpointMetadata.OfType<ApiVersionAttribute>().Any();
                }
                if (docName == "desktop")
                {
                    return actionGroup == "desktop"
                        && !apiDesc.ActionDescriptor.EndpointMetadata.OfType<ApiVersionAttribute>().Any();
                }

                if (docName.StartsWith("external-"))
                {
                    var version = docName.Replace("external-", "");
                    return actionGroup == "external"
                        && apiDesc.GetApiVersion()?.ToString() == version;
                }

                return false;
            });

            // Enable annotations
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http, // Postavi na Http umesto ApiKey
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your Bearer token in the format: Bearer {your_token}",
                }
            );
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Name = "ApiKey",
                Type = SecuritySchemeType.ApiKey,
                Description = "Enter your API key"
            });

            // Add security requirement
            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            new string[] { }
                        },
                }
            );
            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "ApiKey",
                                },
                            },
                            new string[] { }
                        },
                }
            );

            c.EnableAnnotations();
            c.SchemaFilter<EnumSchemaFilter>();
            c.SchemaFilter<DateTimeSchemaFilter>();

            // Enable XML Comments
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var xmlFile = $"{assemblyName}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            // Load XML documentation for the referenced assembly
            foreach (var externalAssemblyName in _referencedAssemblies)
            {
                try
                {
                    var referencedAssembly = Assembly.Load(externalAssemblyName);
                    var referencedXmlFile = $"{referencedAssembly.GetName().Name}.xml";
                    var referencedXmlPath = Path.Combine(AppContext.BaseDirectory, referencedXmlFile);
                    if (File.Exists(referencedXmlPath))
                    {
                        c.IncludeXmlComments(referencedXmlPath);
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Failed to load XML comments for assembly '{externalAssemblyName}'";
                    _logger.LogWarning(ex, errorMessage);
                }
            }
        }
    }
}
