using System.Security.Claims;
using CabaVS.IdentityServer.Web.Constants;
using CabaVS.IdentityServer.Web.Persistence.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace CabaVS.IdentityServer.Web.Initializers;

internal class DbInitializer(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    public async Task Initialize()
    {
        if (await roleManager.FindByNameAsync(Roles.Admin) is null) 
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
        if (await roleManager.FindByNameAsync(Roles.User) is null) 
            await roleManager.CreateAsync(new IdentityRole(Roles.User));

        const string adminEmail = "admin@nomail.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                PhoneNumber = "11111111"
            };
            
            await userManager.CreateAsync(adminUser, "Admin123*");
            await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            
            await userManager.AddClaimsAsync(adminUser, [new Claim(JwtClaimTypes.Role, Roles.Admin)]);
        }
        
        const string userEmail = "user@nomail.com";
        var user = await userManager.FindByEmailAsync(userEmail);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = userEmail,
                Email = userEmail,
                EmailConfirmed = true,
                PhoneNumber = "22222222"
            };
            
            await userManager.CreateAsync(user, "User123*");
            await userManager.AddToRoleAsync(user, Roles.User);

            await userManager.AddClaimsAsync(user, [new Claim(JwtClaimTypes.Role, Roles.User)]);
        }
    }
}