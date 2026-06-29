using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Motivation.IdentityServer.Domain.Base;

namespace Motivation.KpiService.Definitions.OpenApi
{
    /// <summary>
    /// Swagger security configuration scheme
    /// </summary>
    public sealed class OAuth2SecuritySchemeTransformer : IOpenApiDocumentTransformer
    {
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private readonly IHostEnvironment _environment;
        private readonly string? _authServerUrl;
        private readonly string? _serverUrl;
        private readonly IConfiguration _configuration;

        public OAuth2SecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider, IConfiguration configuration, IHostEnvironment environment)
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _configuration = configuration;
            _authServerUrl = configuration.GetSection("AuthServer").GetValue<string>("Url");
            _serverUrl = configuration.GetSection("OpenApi").GetValue<string>("ProdactionUrl");
            _environment = environment;
            ArgumentNullException.ThrowIfNull(_authServerUrl);
        }

        public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            if (_environment.IsProduction() && !string.IsNullOrWhiteSpace(_serverUrl))
            {
                document.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer { Url = _serverUrl }
                    };
            }

            var authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
            if (authenticationSchemes.Any())
            {
                var reference = new OpenApiReference() { Id = "oauth2", Type = ReferenceType.SecurityScheme };
                var scheme = new OpenApiSecurityScheme
                {
                    Reference = reference,
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"{_authServerUrl}/connect/token", UriKind.Absolute),
                            AuthorizationUrl = new Uri($"{_authServerUrl}/connect/authorize", UriKind.Absolute),
                            Scopes = new Dictionary<string, string> { { "api", "Default scope" } }
                        }
                    }
                };

                var requirement = new OpenApiSecurityRequirement
                {
                    { new OpenApiSecurityScheme { Reference = reference, In = ParameterLocation.Cookie, Type = SecuritySchemeType.OAuth2 }, new List<string>() }
                };

                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
                document.SecurityRequirements = [requirement];
                document.Components.SecuritySchemes[scheme.Reference.Id] = scheme;
                document.Info = new()
                {
                    License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://mit-license.org/") },
                    Title = _configuration.GetValue<string>("OpenApi:ServiceName") ?? "Wep-Api",
                    Version = _configuration.GetValue<string>("OpenApi:Version") ?? "Wep-Api version",
                    Description = _configuration.GetValue<string>("OpenApi:Description") ?? "Web-Api description",
                    Contact = new() { Name = "Pesterev Pavel", Url = new("https://github.com/PesterevPavel01"), }
                };
            }
        }
    }
}
