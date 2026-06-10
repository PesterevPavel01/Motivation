using Calabonga.AspNetCore.AppDefinitions;
using Motivation.IdentityServer.Infrastructure.DatabaseInitialization;

namespace Calabonga.Microservice.Module1.Web.Definitions.DataSeeding
{
    /// <summary>
    /// Seeding DbContext for default data for EntityFrameworkCore
    /// </summary>
    public class DatabaseInitializeDefinition : AppDefinition
    {
        /// <summary>
        /// Configure application for current microservice
        /// </summary>
        /// <param name="app"></param>
        public override void ConfigureApplication(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            DatabaseInitializer.Initialize(scope.ServiceProvider);
        }
    }
}
