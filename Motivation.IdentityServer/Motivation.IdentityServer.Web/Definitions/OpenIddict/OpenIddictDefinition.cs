using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.AspNetCore.Hosting.Server;
using Motivation.IdentityServer.Domain.Base;
using Motivation.IdentityServer.Domain.Configuration;
using Motivation.IdentityServer.Infrastructure;
using Motivation.IdentityServer.Web.HostedServices;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using System.Security.Cryptography.X509Certificates;

namespace Motivation.IdentityServer.Web.Definitions.OpenIddict
{
    public class OpenIddictDefinition : AppDefinition
    {
        public override void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddOpenIddict()
                // Register the OpenIddict core components.
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                        .UseDbContext<ApplicationDbContext>()
                        .ReplaceDefaultEntities<Guid>();
                })

                // Register the OpenIddict server components.
                .AddServer(options =>
                {
                    options
                        .AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange()
                        .AllowPasswordFlow()
                        .AllowClientCredentialsFlow()
                        .AllowRefreshTokenFlow();

                    // Enable the token endpoint.
                    options.SetAuthorizationEndpointUris("connect/authorize").RequireProofKeyForCodeExchange() // enable PKCE
                                                                                                               //.SetDeviceEndpointUris("connect/device")
                        .SetIntrospectionEndpointUris("connect/introspect")
                        .SetEndSessionEndpointUris("connect/logout")
                        .SetTokenEndpointUris("connect/token")
                        //.SetVerificationEndpointUris("connect/verify"),
                        .SetUserInfoEndpointUris("connect/userinfo");

                    // Encryption and signing of tokens
                    options
                        .AddEphemeralEncryptionKey() // only for Developing mode
                        .AddEphemeralSigningKey() // only for Developing mode
                        .DisableAccessTokenEncryption(); // only for Developing mode

                    // Mark the "email", "profile" and "roles" scopes as supported scopes.
                    options.RegisterScopes(
                        OpenIddictConstants.Scopes.Email,
                        OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Scopes.Roles,
                        AppData.ScopeApi,
                        "custom",
                        AppData.ScopeSync,
                        AppData.AudienceApi);

                    options.RegisterAudiences(AppData.AudienceApi);

                    var issuerUri = builder.Configuration["AuthServer:IssuerUri"];
                    if (!string.IsNullOrEmpty(issuerUri))
                    {
                        options.SetIssuer(new Uri(issuerUri));
                    }

                    // Register the signing and encryption credentials.
                    var useDevCerts = builder.Configuration.GetValue<bool>("AuthServer:DevCertificates");
                    if (useDevCerts)
                    {
                        options.AddDevelopmentEncryptionCertificate()
                               .AddDevelopmentSigningCertificate();
                    }
                    else
                    {
                        var certPath = builder.Configuration["AuthServer:Certificate:Path"];
                        var certPassword = builder.Configuration["AuthServer:Certificate:Password"];
                        var certBytes = File.ReadAllBytes(certPath);

                        var cert = new X509Certificate2(certBytes, certPassword,
                            X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

                        options.AddEncryptionCertificate(cert)
                               .AddSigningCertificate(cert);
                    }

                    // Register the ASP.NET Core host and configure the ASP.NET Core options.
                    options.UseAspNetCore()
                    .EnableEndSessionEndpointPassthrough()
                    .EnableTokenEndpointPassthrough()
                    .EnableAuthorizationEndpointPassthrough()
                    .DisableTransportSecurityRequirement();


                })

                // Register the OpenIddict validation components.
                .AddValidation(options =>
                {
                    // Import the configuration from the local OpenIddict server instance.
                    options.UseLocalServer();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });

            builder.Services.AddHostedService<OpenIddictWorker>();

            builder.Services.Configure<AuthServerConfiguration>(
                builder.Configuration.GetSection("AuthServer"));
        }
    }
}
