using System.ComponentModel.DataAnnotations;

namespace CabaVS.IdentityServer.Web.Pages.Account.Register;

public sealed class InputModel
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    
    public string? ReturnUrl { get; set; }
}