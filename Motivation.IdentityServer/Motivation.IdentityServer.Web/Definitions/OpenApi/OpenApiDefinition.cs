using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.AspNetCore.Mvc;
using Motivation.IdentityServer.Domain.Base;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Motivation.IdentityServer.Web.Definitions.OpenApi
{
    /// <summary>
    /// Swagger definition for application
    /// </summary>
    public class OpenApiDefinition : AppDefinition
    {
        public const string AppVersion = "1.0";

        private const string _openApiConfig = "/openapi/v1.json";

        public override void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<OAuth2SecuritySchemeTransformer>();
            });
        }

        public override void ConfigureApplication(WebApplication app)
        {
            /*if (!app.Environment.IsDevelopment())
            {
                return;
            }*/

            var userToServiceClientId = app.Configuration["AuthServer:Clients:UserToService:ClientId"] ?? "user-to-service";
            var userToServiceClientSecret = app.Configuration["AuthServer:Clients:UserToService:ClientSecret"];

            app.MapOpenApi();

            app.UseSwaggerUI(settings =>
            {
                settings.SwaggerEndpoint(_openApiConfig, $"{AppData.ServiceName} v.{AppVersion}");

                settings.DocumentTitle = $"{AppData.ServiceName}";
                settings.DefaultModelExpandDepth(0);
                settings.DefaultModelRendering(ModelRendering.Model);
                settings.DefaultModelsExpandDepth(0);
                settings.DocExpansion(DocExpansion.None);
                settings.OAuthScopeSeparator(" ");
                settings.OAuthClientId(userToServiceClientId);
                settings.OAuthClientSecret(userToServiceClientSecret);
                settings.DisplayRequestDuration();
                settings.OAuthUsePkce();
                settings.OAuthAppName(AppData.ServiceName);
            });
        }
    }
}
