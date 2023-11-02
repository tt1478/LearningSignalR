using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SignalRHubs;
using SignalRServer.Hubs;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = "https://localhost:5001";
    options.ClientId = "SignalR2";
    options.ClientSecret = "ClientSecret2";
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
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR().AddStackExchangeRedis("127.0.0.1:6379");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<LearningHub>("/learningHub");

app.Run();
