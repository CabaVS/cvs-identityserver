using Duende.IdentityServer.Models;

namespace CabaVS.IdentityServer.Web.Configuration.Models;

internal sealed class ClientModel
{
    public string ClientId { get; set; } = default!;
    public string[] ClientSecrets { get; set; } = default!;
    public string[] AllowedGrantTypes { get; set; } = default!;
    public string[] AllowedScopes { get; set; } = default!;
    public string[] RedirectUris { get; set; } = default!;
    public string[] PostLogoutRedirectUris { get; set; } = default!;
}

internal static partial class ConfigurationExtensions
{
    public static IEnumerable<Client> LoadClientsFromConfig(
        this IConfiguration configuration,
        string configKey = "Clients")
    {
        var collection = configuration.GetSection(configKey).Get<ClientModel[]>()
                         ?? throw new InvalidOperationException("Clients configuration not found.");
        return collection.Select(x => new Client
        {
            ClientId = x.ClientId,
            ClientSecrets = x.ClientSecrets.Select(secret => new Secret(secret.Sha256())).ToList(),
            AllowedGrantTypes = x.AllowedGrantTypes,
            AllowedScopes = x.AllowedScopes,
            RedirectUris = x.RedirectUris,
            PostLogoutRedirectUris = x.PostLogoutRedirectUris
        });
    }
}