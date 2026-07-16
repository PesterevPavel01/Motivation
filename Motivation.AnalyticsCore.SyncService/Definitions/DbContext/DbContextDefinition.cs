using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.EntityFrameworkCore;
using Motivation.AnalyticsCore.Infrastructure;
using Motivation.AnalyticsCore.Infrastructure.Interceptors;

namespace Motivation.AnaliticsCore.SyncService.Definitions.DbContext
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
            builder.Services.AddScoped<ConvertDomainEventToOutboxMessageInterceptor>();

            var connectionString = builder.Configuration.GetConnectionString("AppDbConnectionString");

            builder.Services.AddDbContext<AnalyticsCoreDbContext>((serviceProvider, options) =>
            {
                options.UseNpgsql(
                    connectionString,
                    o =>
                    {
                        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        o.CommandTimeout(300);
                    })
                    .AddInterceptors(
                        serviceProvider.GetRequiredService<ConvertDomainEventToOutboxMessageInterceptor>(),
                        serviceProvider.GetRequiredService<AuditableDataInterceptor>());
            });
        }
    }
}
