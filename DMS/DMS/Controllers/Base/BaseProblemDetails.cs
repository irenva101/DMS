using CSharpFunctionalExtensions;
using DMS.Shared.Commons;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers.Base
{
    public class BaseProblemDetails : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        protected IActionResult HandleResult<T>(Result<T, ResponseError> result)
        {
            if (result.IsSuccess)
                return Ok(result.Value);

            return result.Error.ErrorCode switch
            {
                ResponseErrorCode.BadRequest or ResponseErrorCode.Database_BadData => BadRequest(CreateProblemDetails(result.Error, HttpContext)),
                ResponseErrorCode.Database_NotFound => NotFound(CreateProblemDetails(result.Error, HttpContext)),
                ResponseErrorCode.Unauthorized => Unauthorized(CreateProblemDetails(result.Error, HttpContext)),
                ResponseErrorCode.Exception => StatusCode(500, CreateProblemDetails(result.Error, HttpContext)),
                _ => StatusCode(500, CreateProblemDetails(result.Error, HttpContext))
            };
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleValidationProblem(FluentValidation.Results.ValidationResult result)
        {
            var errors = result.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );

            var problem = ProblemDetailsFactory.CreateProblemDetails(httpContext: HttpContext,
            statusCode: StatusCodes.Status400BadRequest,
            detail: "One or more fields failed validation.");

            problem.Extensions["errors"] = errors;

            return new ObjectResult(problem) { StatusCode = problem.Status };
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleBadRequest(string error)
        {
            var problem = ProblemDetailsFactory.CreateProblemDetails(httpContext: HttpContext,
            statusCode: StatusCodes.Status400BadRequest,
            detail: error);
            return StatusCode((int)problem.Status!, problem);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ProblemDetails HandleNotFound(ResponseError error)
        {
            var problem = ProblemDetailsFactory.CreateProblemDetails(httpContext: HttpContext,
                statusCode: StatusCodes.Status404NotFound,
                detail: error.Message);
            return problem;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ProblemDetails HandleInternalServerError(ResponseError error)
        {
            var problem = ProblemDetailsFactory.CreateProblemDetails(httpContext: HttpContext,
                statusCode: StatusCodes.Status500InternalServerError,
                detail: error.Message);
            return problem;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleInternalServerError(string error)
        {
            var problem = ProblemDetailsFactory.CreateProblemDetails(httpContext: HttpContext,
                statusCode: StatusCodes.Status500InternalServerError,
                detail: error);
            return StatusCode((int)problem.Status!, problem);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleForbiddenError(string message)
        {
            var problem = ProblemDetailsFactory.CreateProblemDetails(httpContext: HttpContext,
                statusCode: StatusCodes.Status403Forbidden,
                detail: message);

            return StatusCode((int)problem.Status!, problem);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ProblemDetails HandleUnauthorizedError(ResponseError error)
        {
            var problem = ProblemDetailsFactory.CreateProblemDetails(httpContext: HttpContext,
               statusCode: StatusCodes.Status401Unauthorized,
               detail: error.Message);

            return problem;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleUnauthorizedError(string message)
        {
            var problem = ProblemDetailsFactory.CreateProblemDetails(httpContext: HttpContext,
                statusCode: StatusCodes.Status401Unauthorized,
                detail: message);

            return StatusCode((int)problem.Status!, problem);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ProblemDetails CreateProblemDetails(ResponseError error, HttpContext context)
        {
            var problemDetails = ProblemDetailsFactory.CreateProblemDetails(
                httpContext: context,
                statusCode: error.ErrorCode switch
                {
                    ResponseErrorCode.BadRequest or ResponseErrorCode.Database_BadData => StatusCodes.Status400BadRequest,
                    ResponseErrorCode.Database_NotFound => StatusCodes.Status404NotFound,
                    ResponseErrorCode.Unauthorized => StatusCodes.Status401Unauthorized,
                    ResponseErrorCode.Forbidden => StatusCodes.Status403Forbidden,
                    ResponseErrorCode.Exception => StatusCodes.Status500InternalServerError,
                    _ => StatusCodes.Status500InternalServerError
                },

                detail: error.Message
                );

            problemDetails.Extensions["responseCode"] = error.ErrorCode.ToString();
            return problemDetails;
        }
    }
}
