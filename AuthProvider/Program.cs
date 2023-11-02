using AuthProvider;
using AuthProvider.Data;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var assembly = typeof(Program).Assembly.GetName().Name;

var defaultConnectionString = builder.Configuration.GetConnectionString("Default");

SeedData.EnsureSeedData(defaultConnectionString);

builder.Services.AddDbContext<AspNetIdentityDbContext>(options => options.UseSqlServer(defaultConnectionString, b => b.MigrationsAssembly(assembly)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AspNetIdentityDbContext>();

builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;

    options.UserInteraction = new UserInteractionOptions
    {
        LogoutUrl = "/Account/Logout",
        LoginUrl = "/Account/Login",
        LoginReturnUrlParameter = "returnUrl",
    };
})
.AddAspNetIdentity<IdentityUser>()
.AddConfigurationStore(options =>
{
    options.ConfigureDbContext = opt =>
    {
        opt.UseSqlServer(defaultConnectionString, opt => opt.MigrationsAssembly(assembly));
    };
})
.AddOperationalStore(options =>
{
    options.ConfigureDbContext = opt =>
    {
        opt.UseSqlServer(defaultConnectionString, opt => opt.MigrationsAssembly(assembly));
    };
})
.AddDeveloperSigningCredential();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
