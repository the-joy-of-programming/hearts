using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AppIdentity
{
    [Route("auth")]
    public class AuthController : Controller
    {

        private readonly IdentityService identityService;

        public AuthController(IdentityService identityService)
        {
            this.identityService = identityService;
        }

        [HttpGet]
        public string Test()
        {
            return "Ok";
        }

        [HttpGet]
        [Route("challenge")]
        public ChallengeResult Challenge([FromQuery]string providerName)
        {
            var authProps = new AuthenticationProperties
            {
                RedirectUri = $"{Url.Action(nameof(Callback))}?providerName={providerName}",
                Items =
                {
                    { "returnUrl", Url.Action(nameof(Test)) },
                    { "scheme", providerName },
                }
            };            
            var result = Challenge(authProps, new string[] {providerName});
            return result;
        }

        [HttpGet]
        [Route("callback")]
        public async Task<IActionResult> Callback([FromQuery]string providerName)
        {
            var result = await HttpContext.AuthenticateAsync($"pace.{providerName}");
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            var sub = GetSubFromOidcResult(result);

            var user = await identityService.Login(providerName, sub);
            if (user == null)
            {
                return NoContent();
            }
            else
            {
                return NoContent();
            }
        }

        private string GetSubFromOidcResult(AuthenticateResult result)
        {
            return ((ClaimsPrincipal)result.Principal).FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegistrationRequest registrationRequest, [FromQuery]string providerName)
        {
            var result = await HttpContext.AuthenticateAsync($"pace.{providerName}");
            if (result?.Succeeded != true)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "User either has not challenged an external identity provider or the challenge cookie expired");
            }

            var sub = GetSubFromOidcResult(result);
            var registrationResult = await this.identityService.Register(providerName, sub, registrationRequest.DisplayName, registrationRequest.Email);
            if (registrationResult.NewUser == null)
            {
                return BadRequest(new RegistrationResponse { DuplicateEmail = registrationResult.DuplicateEmail, DuplicateDisplayName = registrationResult.DuplicateName, Success = false });
            } else
            {
                return Ok(new RegistrationResponse { Success = true });
            }
        }

    }
}