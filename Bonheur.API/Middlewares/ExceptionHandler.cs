using Bonheur.Utils;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Middlewares
{
    public class ExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {

            var response = exception switch
            {
                ApiException apiEx => new ProblemDetails
                {
                    Status = (int)apiEx.StatusCode,
                    Title = "API Error",
                    Detail = apiEx.Message,
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
                },
                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal server error",
                    Detail = "Internal server error",
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
                }
            };

            httpContext.Response.ContentType = "application/problem+json";
            httpContext.Response.StatusCode = response.Status ?? StatusCodes.Status500InternalServerError;
            
            await httpContext.Response.WriteAsJsonAsync(response);

            return true;
        }
    }

}
