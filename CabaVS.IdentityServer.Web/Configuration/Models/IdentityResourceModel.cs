using Duende.IdentityServer.Models;

namespace CabaVS.IdentityServer.Web.Configuration.Models;

internal sealed class IdentityResourceModel
{
    public string Name { get; set; } = default!;
}

internal static partial class ConfigurationExtensions
{
    public static IEnumerable<IdentityResource> LoadIdentityResourcesFromConfig(
        this IConfiguration configuration,
        string configKey = "IdentityResources")
    {
        var collection = configuration.GetSection(configKey).Get<IdentityResourceModel[]>()
                         ?? throw new InvalidOperationException("Identity Resources configuration not found.");
        return collection.Select(IdentityResource (x) =>
        {
            if (string.IsNullOrWhiteSpace(x.Name))
                throw new InvalidOperationException("Identity Resource Name is required.");

            if (string.Equals(x.Name, "openid", StringComparison.OrdinalIgnoreCase))
                return new IdentityResources.OpenId();
            if (string.Equals(x.Name, "profile", StringComparison.OrdinalIgnoreCase))
                return new IdentityResources.Profile();
            if (string.Equals(x.Name, "email", StringComparison.OrdinalIgnoreCase))
                return new IdentityResources.Email();
            
            throw new InvalidOperationException($"Identity Resource '{x.Name}' is not supported.");
        });
    }
}