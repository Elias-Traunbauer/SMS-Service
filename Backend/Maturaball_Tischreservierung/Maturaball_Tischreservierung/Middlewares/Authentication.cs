using Maturaball_Tischreservierung.Helpers;
using Maturaball_Tischreservierung.Models;
using Maturaball_Tischreservierung.Services;
using System.Net;
using System.Security.Claims;
using TraunisExtensions;
using static Microsoft.AspNetCore.Http.StatusCodes;


namespace Maturaball_Tischreservierung.Middlewares
{
    public class UserAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiConfig _config;
        private readonly SessionBlacklistService sessionBlacklistService;

        public UserAuthenticationMiddleware(RequestDelegate next, ApiConfig configuration, SessionBlacklistService sessionBlacklistService)
        {
            _next = next;
            _config = configuration;
            this.sessionBlacklistService = sessionBlacklistService;
        }

        public async Task Invoke(HttpContext httpContext, JsonWebTokenService jsonWebTokenService, ClientService userService)
        {
            // log everything in the console
            Console.WriteLine($"{httpContext.Request.Method} {httpContext.Request.Path}");
            Console.WriteLine($"Headers: {httpContext.Request.Headers}");
            Console.WriteLine($"Body: {httpContext.Request.Body}");

            // in case of special requests like OPTIONS, we don't want to do anything
            if (httpContext.Request.Method == "OPTIONS")
            {
                await _next(httpContext);
                return;
            }
            if (httpContext.Request.Method == "HEAD")
            {
                await _next(httpContext);
                return;
            }

            var endpoint = httpContext.GetEndpoint();

            if (endpoint.IsNull())
            {
                return;
            }
            if (endpoint.Metadata.Any(x => x is NoAuthenticationRequiredAttribute))
            {
                await _next(httpContext);
                return;
            }

            AuthenticationStatus authenticationStatus = AuthenticationStatus.Unauthenticated;

            httpContext.Items[nameof(HttpContextUserInfo)] = new HttpContextUserInfo();

            string? accessToken = httpContext.Request.Cookies[_config.AccessTokenCookieIdentifier];
            if (accessToken.IsNotNull())
            {
                var claims = jsonWebTokenService.ValidateToken(accessToken);

                if (claims.IsNotNull())
                {
                    SetUserData(httpContext, claims);
                    authenticationStatus = AuthenticationStatus.Authenticated;
                }
            }

            if (authenticationStatus == AuthenticationStatus.Authenticated)
            {
                if (await sessionBlacklistService.
                    IsSessionBlacklistedAsync(
                    httpContext.GetUserInfo().SessionId!))
                {
                    authenticationStatus = AuthenticationStatus.Blacklisted;
                }
            }
            switch (authenticationStatus)
            {
                case AuthenticationStatus.Unauthenticated:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await httpContext.Response.WriteAsJsonAsync(new
                    {
                        Status = Status401Unauthorized,
                        Message = "Unauthenticated"
                    });
                    return;
                case AuthenticationStatus.Blacklisted:
                    httpContext.Response.DeleteCookie(_config.AccessTokenCookieIdentifier);
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await httpContext.Response.WriteAsJsonAsync(new
                    {
                        Status = Status401Unauthorized,
                        Message = "Session blacklisted"
                    });
                    return;
                case AuthenticationStatus.Authenticated:
                    break;
                default:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return;
            }

            await _next(httpContext);
        }

        enum AuthenticationStatus
        {
            Unauthenticated,
            Authenticated,
            Blacklisted,
        }

        private static void SetUserData(HttpContext httpContext, ClaimsPrincipal claims)
        {
            HttpContextUserInfo httpContextUserInfo = new();
            Guid userId = Guid.Parse(claims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string firstName = claims.FindFirst(ClaimTypes.Name)!.Value;
            string lastName = claims.FindFirst(ClaimTypes.Surname)!.Value;
            string email = claims.FindFirst(ClaimTypes.Email)!.Value;
            Guid versionNumber = Guid.Parse(claims.FindFirst(ClaimTypes.Version)!.Value);
            Guid sessionId = Guid.Parse(claims.FindFirst(ClaimTypes.Sid)!.Value);
            string className = claims.FindFirst(ClaimTypes.Actor)!.Value;

            httpContextUserInfo.User = new Client()
            {
                Id = userId,
                Name = firstName,
                Surname = lastName,
                Email = email,
                Version = versionNumber,
                Class = className,
            };
            httpContextUserInfo.SessionId = sessionId;
            httpContext.Items[nameof(HttpContextUserInfo)] = httpContextUserInfo;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthentificationExtensions
    {
        public static IApplicationBuilder UseUserAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserAuthenticationMiddleware>();
        }
    }
}