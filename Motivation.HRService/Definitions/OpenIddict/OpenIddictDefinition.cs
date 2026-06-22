using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.IdentityModel.Tokens;

namespace Motivation.HRService.Definitions.OpenIddict
{
    public class OpenIddictDefinition : AppDefinition
    {
        public override void ConfigureServices(WebApplicationBuilder builder)
        {
            /*var authServerUrl = builder.Configuration.GetValue<string>("AuthServer:Url");
            var clientId = builder.Configuration.GetValue<string>("AuthServer:Clients:UserToService:ClientId");
            var clientSecret = builder.Configuration.GetValue<string>("AuthServer:Clients:UserToService:ClientSecret");*/

            if (builder.Environment.IsDevelopment())
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
}
