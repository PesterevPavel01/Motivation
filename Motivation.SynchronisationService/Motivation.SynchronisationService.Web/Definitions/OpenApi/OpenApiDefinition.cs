using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Motivation.SynchronisationService.Web.Definitions.OpenApi
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
            if (!app.Environment.IsDevelopment())
            {
                return;
            }

            var url = app.Services.GetRequiredService<IConfiguration>().GetValue<string>("AuthServer:Url");

            app.MapOpenApi();

            var userToServiceClient = app.Services.GetRequiredService<IConfiguration>().GetValue<string>("AuthServer:Clients:UserToService:ClientId");
            var userToServiceSecret = app.Services.GetRequiredService<IConfiguration>().GetValue<string>("AuthServer:Clients:UserToService:ClientSecret");
            if (string.IsNullOrWhiteSpace(userToServiceClient))
            {
                throw new InvalidOperationException("ServiceToService:ClientId is not configured");
            }

            app.UseSwaggerUI(settings =>
            {
                settings.SwaggerEndpoint(_openApiConfig, $"Syncronisation service v.{AppVersion}");

                settings.DocumentTitle = "Syncronisation service";
                settings.DefaultModelExpandDepth(0);
                settings.DefaultModelRendering(ModelRendering.Model);
                settings.DefaultModelsExpandDepth(0);
                settings.DocExpansion(DocExpansion.None);
                settings.OAuthScopeSeparator(" ");
                settings.OAuthClientId(userToServiceClient);
                settings.OAuthClientSecret(userToServiceSecret);
                settings.DisplayRequestDuration();
                settings.OAuthAppName("Syncronisation service");
                settings.OAuthUsePkce();
            });
        }
    }
}
