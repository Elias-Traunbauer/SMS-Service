using System.Net;
using static Maturaball_Tischreservierung.UnitOfWork;

namespace DataManagement.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                if (httpContext.Response.HasStarted)
                {
                    // abort
                }
                else
                {
                    // return error and tell client that this has been recorded

                    if (ex is ConcurrencyException)
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

                        await httpContext.Response.WriteAsJsonAsync(new
                        {
                            Status = (int)HttpStatusCode.Conflict,
                            Error = "Please reload the page, and try again. If this error persists, please contact an developer."
                        });

                        return;
                    }
                    else
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        httpContext.Response.Clear();
                        httpContext.Response.StatusCode = 500;
                        httpContext.Response.ContentType = "application/json";
                        await httpContext.Response.WriteAsJsonAsync(new
                        {
                            Status = (int)HttpStatusCode.InternalServerError,
                            Error = "An internal server error has occured. This event was recorded and reported.",
                            DeveloperDetails = ex.Message
                        });
                    }
                }
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}