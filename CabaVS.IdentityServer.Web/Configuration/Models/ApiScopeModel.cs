using Duende.IdentityServer.Models;

namespace CabaVS.IdentityServer.Web.Configuration.Models;

internal sealed class ApiScopeModel
{
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
}

internal static partial class ConfigurationExtensions
{
    public static IEnumerable<ApiScope> LoadApiScopesFromConfig(
        this IConfiguration configuration,
        string configKey = "ApiScopes")
    {
        var collection = configuration.GetSection(configKey).Get<ApiScopeModel[]>()
                         ?? throw new InvalidOperationException("Api Scopes configuration not found.");
        return collection.Select(x => new ApiScope(x.Name, x.DisplayName));
    }
}