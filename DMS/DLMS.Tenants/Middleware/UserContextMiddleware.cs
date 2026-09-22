using DLMS.Tenants.Repositories.UOWs;
using DMS.Shared.Handlers.Model;
using DMS.Shared.Helper;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace DLMS.Tenants.Middleware
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserContext userContext, IUnitOfWorkTenant unitOfWork)
        {

            // JWT token logic
            if (context.User.Identity?.IsAuthenticated == true)
            {
                if (bool.Parse(context.User.Claims.FirstOrDefault(c => c.Type == "isAdmin")?.Value))
                {
                    userContext.UtilityName = HttpContextHelper.EmptyClaim;
                }
                else
                {
                    var utility = context.User.Claims.FirstOrDefault(c => c.Type == "utility")?.Value;
                    var utilityId = Guid.Parse(context.User.Claims.FirstOrDefault(c => c.Type == "utilityId")?.Value);
                    if (!string.IsNullOrEmpty(utility))
                    {
                        // Additional logic for JWT users can go here
                        userContext.UtilityId = utilityId;
                        userContext.UtilityName = utility;
                        var timeZoneClaim = context.User.Claims.FirstOrDefault(c => c.Type == "timeZone")?.Value;
                        userContext.TimeZone = int.TryParse(timeZoneClaim, out var tz) ? tz : 0;
                    }
                }
            }
            // API key logic
            else if (context.Request.Headers.TryGetValue("ApiKey", out var apiKey))
            {
                var parsedApiKey = Guid.TryParse(apiKey, out var apiKeyGuid);

                var utility = await unitOfWork.GetUtilityRepository().GetByApiKey(apiKeyGuid);
                if (utility.IsFailure)
                {
                    await WriteUnauthorizedAsync(context, "Missing or invalid utility claims.");
                    return;
                }
                else
                {
                    userContext.UtilityName = utility.Value.Acronym;
                    userContext.UtilityId = utility.Value.DmsId;
                    userContext.TimeZone = utility.Value.TimeZone;
                }

            }

            // If neither JWT nor API key → just continue without setting UtilityName
            await _next(context);
        }

        private static async Task WriteUnauthorizedAsync(HttpContext context, string message)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var payload = new
                {
                    message = "Unauthorized",
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        }
    }
}
