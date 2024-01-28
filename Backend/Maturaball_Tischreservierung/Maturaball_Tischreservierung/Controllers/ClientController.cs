using Maturaball_Tischreservierung.Helpers;
using Maturaball_Tischreservierung.Middlewares;
using Maturaball_Tischreservierung.Models;
using Maturaball_Tischreservierung.Services;
using Microsoft.AspNetCore.Mvc;

namespace Maturaball_Tischreservierung.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientController : Controller
    {
        private readonly ApiConfig _apiConfig;
        public ClientController(ApiConfig apiConfig)
        {
            _apiConfig = apiConfig;
        }

        [HttpPost]
        [NoAuthenticationRequired]
        public async Task<IActionResult> CreateClient([FromBody] Client client, [FromServices] ClientService clientService, [FromServices] JsonWebTokenService jsonWebTokenService)
        {
            var result = await clientService.CreateClient(client);

            if (result.Status != InternalStatusCode.Success)
                return Ok(result);

            var clientSessionResult = await clientService.CreateClientSession(result.Value!.Id);

            if (clientSessionResult.Status != InternalStatusCode.Success)
                return Ok(clientSessionResult);

            jsonWebTokenService.CreateAccessToken(result.Value!, clientSessionResult.Value!.Id);

            return Ok(new
            {
                Status = 200
            });
        }

        public record AccountAccessRequestPayload(string Email);

        [HttpPost]
        [Route("AccountAccessRequest")]
        [NoAuthenticationRequired]
        public async Task<IActionResult> AccountAccessRequest([FromBody] AccountAccessRequestPayload body, [FromServices] ClientService clientService, [FromServices] JsonWebTokenService jsonWebTokenService)
        {
            var result = await clientService.RequestAccountAccess(body.Email);

            if (result.Status != InternalStatusCode.Success)
                return Ok(result);

            return Ok(new
            {
                Status = 200
            });
        }

        public record AccountAccessCodePayload(string code);

        [HttpPost]
        [Route("AccessCode")]
        [NoAuthenticationRequired]
        public async Task<IActionResult> AccessCode([FromBody] AccountAccessCodePayload body, [FromServices] ClientService clientService, [FromServices] JsonWebTokenService jsonWebTokenService)
        {
            var result = await clientService.UseAccessCode(body.code);

            if (result.Status != InternalStatusCode.Success)
                return Ok(result);

            string jsonWebToken = jsonWebTokenService.CreateAccessToken(result.Value!.Item1, result.Value!.Item2);

            Response.SetCookie(_apiConfig.AccessTokenCookieIdentifier, jsonWebToken, DateTime.Now.AddDays(7), httpOnly: true);

            return Ok();
        }
    }
}
