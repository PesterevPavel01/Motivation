using Calabonga.AspNetCore.AppDefinitions;

namespace Motivation.KpiCalculationService.Definitions.OpenIddict
{
    public class OpenIddictDefinition : AppDefinition
    {
        public override void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services
                .AddOpenIddict()
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });
        }
    }
}
