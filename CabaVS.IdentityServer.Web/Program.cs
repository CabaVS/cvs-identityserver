using CabaVS.IdentityServer.Web.Configuration.Models;
using CabaVS.IdentityServer.Web.Initializers;
using CabaVS.IdentityServer.Web.Persistence;
using CabaVS.IdentityServer.Web.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        configuration.GetConnectionString("SqlDatabase")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services
    .AddIdentityServer(options =>
    {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
        
        options.EmitStaticAudienceClaim = true;

        options.KeyManagement.Enabled = false;
    })
    .AddInMemoryIdentityResources(configuration.LoadIdentityResourcesFromConfig())
    .AddInMemoryApiScopes(configuration.LoadApiScopesFromConfig())
    .AddInMemoryClients(configuration.LoadClientsFromConfig())
    .AddAspNetIdentity<ApplicationUser>()
    .AddDeveloperSigningCredential();

builder.Services.AddRazorPages();

builder.Services.AddScoped<DbInitializer>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    var sp = builder.Services.BuildServiceProvider();
    var dbInitializer = sp.GetRequiredService<DbInitializer>();
    await dbInitializer.Initialize();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.MapRazorPages();

app.Run();