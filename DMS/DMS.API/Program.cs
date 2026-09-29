using DLMS.Tenants;
using DMS.API.Services.GitRevisionService;
using DMS.API.StartupSetup;
using DMS.API.StartupSetup.Swagger;
using DMS.Library;
using DMS.Shared.Config;
using DMS.Shared.Helper;
using Asp.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IGitRevisionService, GitRevisionService>();
// AppConfig mapping
var appConfig = builder.Configuration.Get<DmsConfig>() // AppConfig mapping
    ?? throw new InvalidOperationException("Configuration could not be bound to DmsConfig.");

DmsConfigValidator.Validate(appConfig); // AppConfig validation
builder.Services.AddSingleton(appConfig);

//databases
builder.Services.AddDmsDatabase(appConfig.ConnectionStrings.DmsDb.Replace("{db_name}", "simpas"));
builder.Services.AddSharedDatabase(appConfig.ConnectionStrings.SharedDb.Replace("{db_name}", "shared"));

// Add services.
builder.Services.AddDmsServices(appConfig); //project services
builder.Services.AddTenantServices(); //tenant project services


// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(
                options =>
                {
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                })
            .AddApiExplorer(
                options =>
                {
                    options.SubstituteApiVersionInUrl = true;
                });

builder.Services.AddSwaggerGen(c =>
{
    c.NonNullableReferenceTypesAsRequired();
    c.SchemaFilter<SwaggerHiddenSchemaFilter>();
});
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();

var app = builder.Build();

using (var startupScope = app.Services.CreateScope())
{
    startupScope.ExecutePendingTenantMigrations();
}

app.UseCors("Dms");
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    await next();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.AddSwaggerUiConfiguration();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
