using Calabonga.AspNetCore.AppDefinitions;
using Motivation.IdentityServer.Web.Application.Services;
using Motivation.IdentityServer.Web.Definitions.Authorizations;

namespace Motivation.IdentityServer.Web.Definitions.DependencyContainer
{
    /// <summary>
    /// Dependency container definition
    /// </summary>
    public class ContainerDefinition : AppDefinition
    {
        public override void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IAccountService, AccountService>();
            builder.Services.AddTransient<ApplicationUserClaimsPrincipalFactory>();
        }
    }
}