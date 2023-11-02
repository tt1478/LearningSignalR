using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SignalRServer.Hubs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRServerUnitTest
{
    public sealed class UnitTestHostService
    {
        private UnitTestHostService()
        {
        }
        private static UnitTestHostServiceModel? _instance;
        public static async Task<UnitTestHostServiceModel> GetInstanse()
        {
            if (_instance == null)
            {
                _instance = new UnitTestHostServiceModel() { TestServer = SetWebHostBuilder(), AccessToken = await GetAccessToken() };
            }
            return _instance;
        }
        private static TestServer SetWebHostBuilder()
        {
            var webHostBuilder = new WebHostBuilder().ConfigureServices(services =>
            {
                services.AddSignalR();
                JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.ClientId = "SignalR1";
                    options.ClientSecret = "ClientSecret1";
                    options.ResponseType = "code";
                    options.CallbackPath = "/signin-oidc";
                    options.SaveTokens = true;
                    options.RequireHttpsMetadata = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                    options.RequireHttpsMetadata = false;
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var path = context.HttpContext.Request.Path;
                            if (path.StartsWithSegments("/learningHub"))
                            {
                                // Attempt to get a token from a query sting used by WebSocket
                                var accessToken = context.Request.Query["access_token"];

                                // If not present, extract the token from Authorization header
                                if (string.IsNullOrWhiteSpace(accessToken))
                                {
                                    accessToken = context.Request.Headers["Authorization"]
                                        .ToString()
                                        .Replace("Bearer ", "");
                                }

                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            })
            .Configure(app =>
            {
                app.UseRouting();

                app.UseAuthentication();

                app.UseAuthorization();

                app.UseEndpoints(endPoints =>
                {

                    endPoints.MapHub<LearningHub>("/learningHub");
                });
            });
            return new TestServer(webHostBuilder);
        }

        private static async Task<string> GetAccessToken()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
            }
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "unitTesting",
                ClientSecret = "ClientSecret1",
                Scope = "UnitTesting"
            });
            return tokenResponse.AccessToken ?? string.Empty;
        }
    }
}
