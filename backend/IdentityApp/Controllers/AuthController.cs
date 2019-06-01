using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp
{
    [Route("auth")]
    public class AuthController : Controller
    {
        const string ProviderNameCookie = "Pace.Identity.Registration.ProviderName";

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

            var sub = GetIdentifierFromAuthResult(result);

            var user = await identityService.Login(providerName, sub);
            if (user == null)
            {
                HttpContext.Response.Cookies.Append(
                    ProviderNameCookie,
                    providerName,
                    new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        HttpOnly = true,
                        MaxAge = TimeSpan.FromHours(24),
                        IsEssential = true // User hasn't registered yet, can't prompt for GDPR until they do
                    }
                );
                return NoContent();
            }
            else
            {
                return Json(user);
            }
        }

        private string GetIdentifierFromAuthResult(AuthenticateResult result)
        {
            return ((ClaimsPrincipal)result.Principal).FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private string GetEmailFromAuthResult(AuthenticateResult result)
        {
            return ((ClaimsPrincipal)result.Principal).FindFirst(ClaimTypes.Email)?.Value;
        }

        private string GetNameFromAuthResult(AuthenticateResult result)
        {
            return ((ClaimsPrincipal)result.Principal).FindFirst("name")?.Value;
        }

        [HttpPost]
        [Route("users/noone")]
        public async Task<IActionResult> LoginAsNoOne()
        {
            var user = await identityService.LoginAsNoOne();
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, PrincipalFromUser(user));
            return Json(user);
        }

        [HttpDelete]
        [Route("users/current")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return NoContent();
        }

        [HttpGet]
        [Route("users/current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Succeeded != true)
            {
                return NoContent();
            }
            var userId = int.Parse(GetIdentifierFromAuthResult(result));
            var displayName = GetNameFromAuthResult(result);
            var email = GetEmailFromAuthResult(result);
            var user = new User { Email = email, DisplayName = displayName, Id = userId };
            return Json(user);
        }

        [HttpGet]
        [Route("registration/pending")]
        public async Task<IActionResult> GetPendingRegistration()
        {
            var providerName = HttpContext.Request.Cookies[ProviderNameCookie];
            if (providerName == null)
            {
                return NoContent();
            }
            var result = await HttpContext.AuthenticateAsync($"pace.{providerName}");
            if (result?.Succeeded != true)
            {
                return NoContent();
            }
            var email = GetEmailFromAuthResult(result);
            var name = GetNameFromAuthResult(result);
            return Json(new RegistrationRequest
            {
                DisplayName = name,
                Email = email
            });
        }

        [HttpPost]
        [Route("registration")]
        public async Task<IActionResult> Register([FromBody]RegistrationRequest registrationRequest)
        {
            var providerName = HttpContext.Request.Cookies[ProviderNameCookie];
            if (providerName == null)
            {
                throw new ApiException(HttpStatusCode.Unauthorized, "User has not logged in recently, need to re-issue challnege");
            }
            var result = await HttpContext.AuthenticateAsync($"pace.{providerName}");
            if (result?.Succeeded != true)
            {
                throw new ApiException(HttpStatusCode.Unauthorized, "User either has not challenged an external identity provider or the challenge cookie expired");
            }

            var sub = GetIdentifierFromAuthResult(result);
            var registrationResult = await this.identityService.Register(providerName, sub, registrationRequest.DisplayName, registrationRequest.Email);
            if (registrationResult.NewUser == null)
            {
                return BadRequest(new RegistrationResponse { DuplicateEmail = registrationResult.DuplicateEmail, DuplicateDisplayName = registrationResult.DuplicateName, Success = false });
            } else
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, PrincipalFromUser(registrationResult.NewUser));
                return Ok(new RegistrationResponse { Success = true });
            }
        }

        private ClaimsPrincipal PrincipalFromUser(User user)
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim("name", user.DisplayName.ToString()),
                        new Claim(ClaimTypes.Email, user.Email.ToString())
                    }
                )
            );
        }

    }
}