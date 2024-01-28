using Maturaball_Tischreservierung.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Maturaball_Tischreservierung.Services
{
    public class JsonWebTokenService
    {
        private readonly ApiConfig _configuration;

        public JsonWebTokenService(ApiConfig config)
        {
            _configuration = config;
        }

        /// <summary>
        /// Generates a new access token for a user
        /// </summary>
        /// <param name="user">The user to generate the token for</param>
        /// <returns></returns>
        public string CreateAccessToken(Client user, Guid sessionId)
        {
            var keyBytes = Convert.FromBase64String(_configuration.Secret);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
            //Guid userId = Guid.Parse(claims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            //string username = claims.FindFirst(ClaimTypes.GivenName)!.Value;
            //string firstName = claims.FindFirst(ClaimTypes.Name)!.Value;
            //string lastName = claims.FindFirst(ClaimTypes.Surname)!.Value;
            //string email = claims.FindFirst(ClaimTypes.Email)!.Value;
            //int versionNumber = int.Parse(claims.FindFirst(ClaimTypes.Version)!.Value);
            //Guid sessionId = Guid.Parse(claims.FindFirst(ClaimTypes.Sid)!.Value);
            //UserPermission userPermission = (UserPermission)int.Parse(claims.FindFirst(ClaimTypes.Role)!.Value);
            
            var token = new JwtSecurityToken(_configuration.Issuer,
              null,
              new List<Claim>()
              {
                  new(ClaimTypes.NameIdentifier,    user.Id.ToString()),
                  new(ClaimTypes.Name,              user.Name),
                  new(ClaimTypes.Surname,           user.Surname),
                  new(ClaimTypes.Email,             user.Email),
                  new(ClaimTypes.Version,           user.Version.ToString()),
                  new(ClaimTypes.Actor,             user.Class),
                  new(ClaimTypes.Sid, sessionId.ToString()),
              },
              expires: DateTime.UtcNow.Add(_configuration.AccessTokenLifetime),
              signingCredentials: credentials);

            string writtenToken = new JwtSecurityTokenHandler().WriteToken(token);
            return writtenToken;
        }

        /// <summary>
        /// Validates a token and returns the principal
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                return new JwtSecurityTokenHandler().ValidateToken(token, GetValidationParameters(), out _);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Cached token validation parameters
        /// </summary>
        private TokenValidationParameters? tokenValidationParameters;

        private TokenValidationParameters GetValidationParameters()
        {
            tokenValidationParameters ??= new()
            {
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidIssuer = _configuration.Issuer,
                ClockSkew = TimeSpan.FromMilliseconds(0),
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration.Secret))
            };

            return tokenValidationParameters;
        }
    }
}