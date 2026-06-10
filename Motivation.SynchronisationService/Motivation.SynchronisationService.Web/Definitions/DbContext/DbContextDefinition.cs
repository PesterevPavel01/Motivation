using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.EntityFrameworkCore;
using Motivation.Infrastructure;
using Motivation.Infrastructure.Interceptors;

namespace Motivation.SynchronisationService.Web.Definitions.DbContext
{
    /// <summary>
    /// ASP.NET Core services registration and configurations
    /// </summary>
    public class DbContextDefinition : AppDefinition
    {
        /// <summary>
        /// Configure services for current microservice
        /// </summary>
        /// <param name="builder"></param>
        public override void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<AuditableDataInterceptor>();

            var connectionString = builder.Configuration.GetConnectionString("AppDbConnectionString");

            builder.Services.AddDbContext<MotivationDbContext>((serviceProvider, options) =>
            {
                options.UseNpgsql(
                    connectionString,
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                        .AddInterceptors(
                            serviceProvider.GetRequiredService<AuditableDataInterceptor>());
            });
        }
    }
}
