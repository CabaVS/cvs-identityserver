using System.Security.Claims;
using CabaVS.IdentityServer.Web.Constants;
using CabaVS.IdentityServer.Web.Persistence.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CabaVS.IdentityServer.Web.Pages.Account.Register;

[SecurityHeaders]
[AllowAnonymous]
public class Index(
    ILogger<Index> logger,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = default!;
    
    public IActionResult OnGet(string? returnUrl)
    {
        Input = new InputModel { ReturnUrl = returnUrl };
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = new ApplicationUser
        {
            UserName = Input.Username,
            Email = Input.Email,
            EmailConfirmed = true
        };
        
        var userCreationResult = await userManager.CreateAsync(user, Input.Password);
        if (!userCreationResult.Succeeded)
        {
            logger.LogWarning(
                "User creation failed. Errors: {Errors}.",
                string.Join("; ", userCreationResult.Errors.Select(x => $"[Code: {x.Code}, Description: {x.Description}]")));
            return Page();
        }
        
        var roleAssignmentResult = await userManager.AddToRoleAsync(user, Roles.User);
        if (!roleAssignmentResult.Succeeded)
        {
            logger.LogWarning(
                "Role assignment failed. Errors: {Errors}.",
                string.Join("; ", roleAssignmentResult.Errors.Select(x => $"[Code: {x.Code}, Description: {x.Description}]")));
            return Page();
        }
        
        var claimsCreationResult = await userManager.AddClaimsAsync(
            user,
            [
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.Name, user.UserName),
                new Claim(JwtClaimTypes.Role, Roles.User)
            ]);
        if (!claimsCreationResult.Succeeded)
        {
            logger.LogWarning(
                "Claims creation failed. Errors: {Errors}.",
                string.Join("; ", claimsCreationResult.Errors.Select(x => $"[Code: {x.Code}, Description: {x.Description}]")));
            return Page();
        }
        
        var loginResult = await signInManager.PasswordSignInAsync(user.UserName, Input.Password, true, false);
        if (!loginResult.Succeeded)
        {
            logger.LogWarning(
                "Password signin failed. IsNotAllowed: {IsNotAllowed}. IsLockedOut: {IsLockedOut}. RequiresTwoFactor: {RequiresTwoFactor}.",
                loginResult.IsNotAllowed,
                loginResult.IsLockedOut,
                loginResult.RequiresTwoFactor);
            return Page();
        }
        
        logger.LogInformation(
            "User registration succeeded. Email: {Email}, UserName: {UserName}.",
            user.Email,
            user.UserName);
        return Redirect("~/");
    }
}