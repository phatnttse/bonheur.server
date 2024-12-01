using Bonheur.Utils;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Bonheur.API.Configurations
{
    public static class OidcServerConfig
    {
        public const string ServerName = "Bonheur API";
        public const string QuickAppClientID = "bonheur_spa";
        public const string SwaggerClientID = "swagger_ui";

        public static async Task RegisterClientApplicationsAsync(IServiceProvider provider)
        {
            var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();

            // Angular SPA Client
            if (await manager.FindByClientIdAsync(QuickAppClientID) is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = QuickAppClientID,
                    ClientType = ClientTypes.Public,
                    DisplayName = "Bonheur SPA",
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.Password,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Phone,
                        Permissions.Scopes.Address,
                        Permissions.Scopes.Roles,
                        Permissions.Prefixes.GrantType + Constants.GrantTypes.ASSERTION                    
                    }
                });
            }

            // Swagger UI Client
            if (await manager.FindByClientIdAsync(SwaggerClientID) is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = SwaggerClientID,
                    ClientType = ClientTypes.Public,
                    DisplayName = "Swagger UI",
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.Password
                    }
                });
            }
        }
    }
}