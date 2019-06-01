using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace IdentityApp
{
    public class LoginAndRegisterTests : IdentityApiTest
    {
        private const string KeyStoreJson = @"{
            ""keys"": [
                {
                    ""kty"":""RSA"",
                    ""n"":""0vx7agoebGcQSuuPiLJXZptN9nndrQmbXEps2aiAFbWhM78LhWx4cbbfAAtVT86zwu1RK7aPFFxuhDR1L6tSoc_BJECPebWKRXjBZCiFV4n3oknjhMstn64tZ_2W-5JsGY4Hc5n9yBXArwl93lqt7_RN5w6Cf0h4QyQ5v-65YGjQR0_FDW2QvzqY368QQMicAtaSqzs8KJZgnYb9c7d0zgdAZHzu6qMQvRL5hajrn1n91CbOpbISD08qNLyrdkt-bFTWhAI4vMQFh6WeZu0fM4lFd2NcRwr3XPksINHaQ-G_xBniIqbw0Ls1jF44-csFCur-kEgU8awapJzKnqDKgw"",
                    ""e"":""AQAB"",
                    ""d"":""X4cTteJY_gn4FYPsXB8rdXix5vwsg1FLN5E3EaG6RJoVH-HLLKD9M7dx5oo7GURknchnrRweUkC7hT5fJLM0WbFAKNLWY2vv7B6NqXSzUvxT0_YSfqijwp3RTzlBaCxWp4doFk5N2o8Gy_nHNKroADIkJ46pRUohsXywbReAdYaMwFs9tv8d_cPVY3i07a3t8MN6TNwm0dSawm9v47UiCl3Sk5ZiG7xojPLu4sbg1U2jx4IBTNBznbJSzFHK66jT8bgkuqsk0GjskDJk19Z4qwjwbsnn4j2WBii3RL-Us2lGVkY8fkFzme1z0HbIkfz0Y6mqnOYtqc0X4jfcKoAC8Q"",
                    ""p"":""83i-7IvMGXoMXCskv73TKr8637FiO7Z27zv8oj6pbWUQyLPQBQxtPVnwD20R-60eTDmD2ujnMt5PoqMrm8RfmNhVWDtjjMmCMjOpSXicFHj7XOuVIYQyqVWlWEh6dN36GVZYk93N8Bc9vY41xy8B9RzzOGVQzXvNEvn7O0nVbfs"",
                    ""q"":""3dfOR9cuYq-0S-mkFLzgItgMEfFzB2q3hWehMuG0oCuqnb3vobLyumqjVZQO1dIrdwgTnCdpYzBcOfW5r370AFXjiWft_NGEiovonizhKpo9VVS78TzFgxkIdrecRezsZ-1kYd_s1qDbxtkDEgfAITAG9LUnADun4vIcb6yelxk"",
                    ""dp"":""G4sPXkc6Ya9y8oJW9_ILj4xuppu0lzi_H7VTkS8xj5SdX3coE0oimYwxIi2emTAue0UOa5dpgFGyBJ4c8tQ2VF402XRugKDTP8akYhFo5tAA77Qe_NmtuYZc3C3m3I24G2GvR5sSDxUyAN2zq8Lfn9EUms6rY3Ob8YeiKkTiBj0"",
                    ""dq"":""s9lAH9fggBsoFR8Oac2R_E2gw282rT2kGOAhvIllETE1efrA6huUUvMfBcMpn8lqeW6vzznYY5SSQF7pMdC_agI3nG8Ibp1BUb0JUiraRNqUfLhcQb_d9GF4Dh7e74WbRsobRonujTYN1xCaP6TO61jvWrX-L18txXw494Q_cgk"",
                    ""qi"":""GyM_p6JrXySiz1toFgKbWV-JdI3jQ4ypu9rbMWx3rQJBfmt0FoYzgUIZEVFEcOqwemRN81zoDAaa-Bk0KWNGDjJHZDdDmFhW3AN7lI-puxk_mHZGJ11rxyR8O55XLSe3SPmRfKwZI6yU24ZxvQKFYItdldUKGzO6Ia6zTKhAVRU"",
                    ""alg"":""RS256"",
                    ""kid"":""mykey""
                }
            ]
        }";

        private const string WellKnownConfigurationJson = @"{
            ""issuer"": ""https://baseurl"",
            ""authorization_endpoint"": ""https://baseurl/o/oauth2/v2/auth"",
            ""token_endpoint"": ""https://baseurl/token"",
            ""userinfo_endpoint"": ""https://baseurl/v1/userinfo"",
            ""revocation_endpoint"": ""https://baseurl/revoke"",
            ""jwks_uri"": ""https://baseurl/oauth2/v3/certs"",
            ""response_types_supported"": [
                ""code"",
                ""token"",
                ""id_token"",
                ""code token"",
                ""code id_token"",
                ""token id_token"",
                ""code token id_token"",
                ""none""
            ],
            ""subject_types_supported"": [
                ""public""
            ],
            ""id_token_signing_alg_values_supported"": [
                ""RS256""
            ],
            ""scopes_supported"": [
                ""openid"",
                ""email"",
                ""profile""
            ],
            ""token_endpoint_auth_methods_supported"": [
               ""client_secret_post"",
               ""client_secret_basic""
            ],
            ""claims_supported"": [
                ""aud"",
                ""email"",
                ""email_verified"",
                ""exp"",
                ""family_name"",
                ""given_name"",
                ""iat"",
                ""iss"",
                ""locale"",
                ""name"",
                ""picture"",
                ""sub""
            ],
            ""code_challenge_methods_supported"": [
                ""plain"",
                ""S256""
            ]
        }";

        private const string MockProviderOneName = "mockprovider";
        private const string MockProviderTwoName = "mockprovidertoo";

        private readonly RSAParameters rsaParameters;

        public LoginAndRegisterTests()
        {
            var jsonWebKey = new JsonWebKey(KeyStoreJson);
            this.rsaParameters = new RSAParameters
            {
                D = Base64UrlEncoder.DecodeBytes("X4cTteJY_gn4FYPsXB8rdXix5vwsg1FLN5E3EaG6RJoVH-HLLKD9M7dx5oo7GURknchnrRweUkC7hT5fJLM0WbFAKNLWY2vv7B6NqXSzUvxT0_YSfqijwp3RTzlBaCxWp4doFk5N2o8Gy_nHNKroADIkJ46pRUohsXywbReAdYaMwFs9tv8d_cPVY3i07a3t8MN6TNwm0dSawm9v47UiCl3Sk5ZiG7xojPLu4sbg1U2jx4IBTNBznbJSzFHK66jT8bgkuqsk0GjskDJk19Z4qwjwbsnn4j2WBii3RL-Us2lGVkY8fkFzme1z0HbIkfz0Y6mqnOYtqc0X4jfcKoAC8Q"),
                DP = Base64UrlEncoder.DecodeBytes("G4sPXkc6Ya9y8oJW9_ILj4xuppu0lzi_H7VTkS8xj5SdX3coE0oimYwxIi2emTAue0UOa5dpgFGyBJ4c8tQ2VF402XRugKDTP8akYhFo5tAA77Qe_NmtuYZc3C3m3I24G2GvR5sSDxUyAN2zq8Lfn9EUms6rY3Ob8YeiKkTiBj0"),
                DQ = Base64UrlEncoder.DecodeBytes("s9lAH9fggBsoFR8Oac2R_E2gw282rT2kGOAhvIllETE1efrA6huUUvMfBcMpn8lqeW6vzznYY5SSQF7pMdC_agI3nG8Ibp1BUb0JUiraRNqUfLhcQb_d9GF4Dh7e74WbRsobRonujTYN1xCaP6TO61jvWrX-L18txXw494Q_cgk"),
                Exponent = Base64UrlEncoder.DecodeBytes("AQAB"),
                InverseQ = Base64UrlEncoder.DecodeBytes("GyM_p6JrXySiz1toFgKbWV-JdI3jQ4ypu9rbMWx3rQJBfmt0FoYzgUIZEVFEcOqwemRN81zoDAaa-Bk0KWNGDjJHZDdDmFhW3AN7lI-puxk_mHZGJ11rxyR8O55XLSe3SPmRfKwZI6yU24ZxvQKFYItdldUKGzO6Ia6zTKhAVRU"),
                Modulus = Base64UrlEncoder.DecodeBytes("0vx7agoebGcQSuuPiLJXZptN9nndrQmbXEps2aiAFbWhM78LhWx4cbbfAAtVT86zwu1RK7aPFFxuhDR1L6tSoc_BJECPebWKRXjBZCiFV4n3oknjhMstn64tZ_2W-5JsGY4Hc5n9yBXArwl93lqt7_RN5w6Cf0h4QyQ5v-65YGjQR0_FDW2QvzqY368QQMicAtaSqzs8KJZgnYb9c7d0zgdAZHzu6qMQvRL5hajrn1n91CbOpbISD08qNLyrdkt-bFTWhAI4vMQFh6WeZu0fM4lFd2NcRwr3XPksINHaQ-G_xBniIqbw0Ls1jF44-csFCur-kEgU8awapJzKnqDKgw"),
                P = Base64UrlEncoder.DecodeBytes("83i-7IvMGXoMXCskv73TKr8637FiO7Z27zv8oj6pbWUQyLPQBQxtPVnwD20R-60eTDmD2ujnMt5PoqMrm8RfmNhVWDtjjMmCMjOpSXicFHj7XOuVIYQyqVWlWEh6dN36GVZYk93N8Bc9vY41xy8B9RzzOGVQzXvNEvn7O0nVbfs"),
                Q = Base64UrlEncoder.DecodeBytes("3dfOR9cuYq-0S-mkFLzgItgMEfFzB2q3hWehMuG0oCuqnb3vobLyumqjVZQO1dIrdwgTnCdpYzBcOfW5r370AFXjiWft_NGEiovonizhKpo9VVS78TzFgxkIdrecRezsZ-1kYd_s1qDbxtkDEgfAITAG9LUnADun4vIcb6yelxk")
            };
        }

        private static byte[] FromBase64Url(string url)
        {
            return Base64UrlEncoder.DecodeBytes(url);
        }

        protected override void PreStartup()
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            Environment.SetEnvironmentVariable("Identity__Providers__0__Name", MockProviderOneName);
            Environment.SetEnvironmentVariable("Identity__Providers__0__BaseUrl", "https://baseurl");
            Environment.SetEnvironmentVariable("Identity__Providers__0__ClientId", "client-id");
            Environment.SetEnvironmentVariable("Identity__Providers__0__ClientSecret", "client-secret");
            Environment.SetEnvironmentVariable("Identity__Providers__1__Name", MockProviderTwoName);
            Environment.SetEnvironmentVariable("Identity__Providers__1__BaseUrl", "https://baseurl");
            Environment.SetEnvironmentVariable("Identity__Providers__1__ClientId", "client-id");
            Environment.SetEnvironmentVariable("Identity__Providers__1__ClientSecret", "client-secret");
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("https://baseurl/.well-known/openid-configuration")
              .Respond("application/json", WellKnownConfigurationJson);
            mockHttp.When("https://baseurl/oauth2/v3/certs")
              .Respond("application/json", KeyStoreJson);
            Startup.MockOidcBackchannel(mockHttp);
        }

        [TearDown]
        public void Teardown()
        {
            Startup.ClearMockOidcBackchannel();
        }

        public class GivenNothing : LoginAndRegisterTests
        {

            [Test]
            public Task ShouldNotBeLoggedIn()
            {
                return GetAsync<object>("/auth/users/current", HttpStatusCode.NoContent);
            }

            [Test]
            public async Task ShouldNotAllowRegistrationIfNotLoggedIn()
            {
                var registrationRequest = new RegistrationRequest
                {
                    DisplayName = "abc",
                    Email = "abc"
                };
                var err = await PostAsync<RegistrationRequest, Error>("/auth/registration", registrationRequest, HttpStatusCode.Unauthorized);
                Assert.IsNotEmpty(err.Message);
            }

            [Test]
            public async Task ShouldNotShowAnyPendingRegistration()
            {
                await GetAsync<RegistrationResponse>("/auth/registration/pending", HttpStatusCode.NoContent);
            }

        }

        private string CreateJwtToken(string nonce, ChallengeSimulationParameters parameters)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new RsaSecurityKey(rsaParameters);
            var currentTime = DateTime.Now;
            var future = currentTime.AddHours(1);
            var notBefore = currentTime.AddMinutes(-5);
            var subject = new ClaimsIdentity(new[] {
                new Claim("sub", parameters.ResponseSub),
                new Claim("nonce", nonce)
            });

            if (parameters.ResponseName != null)
            {
                subject.AddClaim(new Claim("name", parameters.ResponseName));
            }
            
            if (parameters.ResponseEmail != null)
            {
                subject.AddClaim(new Claim("email", parameters.ResponseEmail));
            }

            var token = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = "https://baseurl",
                IssuedAt = currentTime,
                Audience = "client-id",
                Expires = future,
                NotBefore = notBefore,
                Subject = subject,
                SigningCredentials = new SigningCredentials(key, "RS256")
            });
            return handler.WriteToken(token);
        }

        private async Task SimulateSuccessfulChallenge(ChallengeSimulationParameters parameters)
        {
            var challengeResponse = await RawGetAsync($"/auth/challenge?providerName={parameters.ProviderName}");
            var redirectUrl = challengeResponse.Headers.Location;
            Assert.AreEqual(HttpStatusCode.Redirect, challengeResponse.StatusCode);
            var redirectQueryParams = HttpUtility.ParseQueryString(redirectUrl.Query);
            var state = redirectQueryParams["state"];
            var nonce = redirectQueryParams["nonce"];
            var formData = new Dictionary<string, string>();
            formData["id_token"] = CreateJwtToken(nonce, parameters);
            formData["state"] = state;
            var internalCallbackResponse = await RawPostAsync("/auth/internal-callback", formData);
            Assert.AreEqual(HttpStatusCode.Redirect, internalCallbackResponse.StatusCode);
            Assert.AreEqual($"/auth/callback?providerName={parameters.ProviderName}", internalCallbackResponse.Headers.Location.ToString());
            var callbackResponse = await RawGetAsync($"/auth/callback?providerName={parameters.ProviderName}");
            Assert.AreEqual(HttpStatusCode.NoContent, callbackResponse.StatusCode);
        }

        protected async Task Register(string displayName, string email)
        {
            var registrationRequest = new RegistrationRequest
            {
                DisplayName = displayName,
                Email = email
            };
            var registrationResponse = await PostAsync<RegistrationRequest, RegistrationResponse>("/auth/registration", registrationRequest);
            Assert.NotNull(registrationResponse);
            Assert.AreEqual(true, registrationResponse.Success);
            Assert.False(registrationResponse.DuplicateEmail);
            Assert.False(registrationResponse.DuplicateDisplayName);
        }

        [TestFixture]
        public class GivenSuccessfulExternalLogin : LoginAndRegisterTests
        {
            [SetUp]
            public async Task Setup()
            {
                await SimulateSuccessfulChallenge(new ChallengeSimulationParameters
                {
                    ProviderName = MockProviderOneName,
                    ResponseName = "username",
                    ResponseSub = "user-id",
                    ResponseEmail = "user-email"
                });
            }

            [Test]
            public async Task ShouldShowPendingRegistration()
            {
                var pendingRegistration = await GetAsync<RegistrationRequest>("/auth/registration/pending");
                Assert.NotNull(pendingRegistration);
                Assert.AreEqual("username", pendingRegistration.DisplayName);
                Assert.AreEqual("user-email", pendingRegistration.Email);
            }

            [Test]
            public async Task ShouldAllowMeToRegister()
            {
                await Register("mydude", "email@domain.com");
            }

            [Test]
            public Task ShouldNotBeLoggedIn()
            {
                return GetAsync<object>("/auth/users/current", HttpStatusCode.NoContent);
            }

        }

        [TestFixture]
        public class GivenJustRegisteredUser : LoginAndRegisterTests
        {
            [SetUp]
            public async Task Setup()
            {
                await SimulateSuccessfulChallenge(new ChallengeSimulationParameters
                {
                    ProviderName = MockProviderOneName,
                    ResponseName = "username",
                    ResponseSub = "user-id",
                    ResponseEmail = "user-email"
                });
                await Register("mydude", "email@domain.com");
            }

            [Test]
            public async Task ShouldBeLoggedIn()
            {
                var user = await GetAsync<User>("/auth/users/current");
                Assert.NotNull(user);
                Assert.AreEqual("mydude", user.DisplayName);
                Assert.AreEqual("email@domain.com", user.Email);
            }

            [Test]
            public async Task ShouldAllowLogout()
            {
                await DeleteAsync("/auth/users/current");
                await GetAsync<object>("/auth/users/current", HttpStatusCode.NoContent);
            }
        }

        [TestFixture]
        public class GivenRegisteredButLoggedOut : LoginAndRegisterTests
        {
            [SetUp]
            public async Task Setup()
            {
                await SimulateSuccessfulChallenge(new ChallengeSimulationParameters
                {
                    ProviderName = MockProviderOneName,
                    ResponseName = "username",
                    ResponseSub = "user-id",
                    ResponseEmail = "user-email"
                });
                await Register("mydude", "email@domain.com");
                await DeleteAsync("/auth/users/current");
            }

            [Test]
            public async Task ChallengeShouldLogBackIn()
            {
                await SimulateSuccessfulChallenge(new ChallengeSimulationParameters
                {
                    ProviderName = MockProviderOneName,
                    ResponseSub = "user-id"
                });
                var user = await GetAsync<User>("/auth/users/current");
                Assert.NotNull(user);
                Assert.AreEqual("mydude", user.DisplayName);
                Assert.AreEqual("email@domain.com", user.Email);
            }

        }

    }
}