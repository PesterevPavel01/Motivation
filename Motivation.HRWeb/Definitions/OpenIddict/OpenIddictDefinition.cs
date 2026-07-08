using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Motivation.HRWeb.Extensions;

namespace Motivation.HRWeb.Definitions.OpenIddict;

public class OpenIddictDefinition : AppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(AppData.OidcSchemeName)
            .AddOpenIdConnect(AppData.OidcSchemeName, options =>
            {
                builder.Configuration.GetSection("OpenIDConnectSettings").Bind(options);
                options.MetadataAddress = builder.Configuration["OpenIDConnectSettings:MetadataAddress"];
                options.CallbackPath = "/signin-oidc";
                options.SignedOutCallbackPath = "/signout-callback-oidc";
                options.SignedOutRedirectUri = "/";
                options.CorrelationCookie.Path = "/";
                options.NonceCookie.Path = "/";
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                options.CorrelationCookie.HttpOnly = true;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.ResponseMode = OpenIdConnectResponseMode.Query;
                options.RequireHttpsMetadata = false;
                //options.SaveTokens = true;
                options.SaveTokens = false;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.MapInboundClaims = false;
                options.Scope.Add("offline_access");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };
                options.BackchannelHttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                options.Events = new OpenIdConnectEvents
                {
                    OnRemoteFailure = context =>
                    {
                        context.Response.Redirect("/authentication/login");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    },
                    OnTicketReceived = context =>
                    {
                        if (context is null || context.Properties is null || context.Properties.Items is null)
                        {
                            return Task.CompletedTask;
                        }

                        var properties = new AuthenticationProperties(context.Properties.Items);
                        properties.StoreTokens(context.Properties.GetTokens().Where(token => token switch
                        {
                            { Name: "access_token" or "refresh_token" } => true,
                            _ => false
                        }));

                        return Task.CompletedTask;
                    },
                    OnRedirectToIdentityProvider = context =>
                    {
                        var authority = context.Options.Authority?.TrimEnd('/');
                        if (!string.IsNullOrEmpty(authority))
                        {
                            context.ProtocolMessage.IssuerAddress = authority + "/connect/authorize";
                        }
                        return Task.CompletedTask;
                    },
                    OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        var authority = context.Options.Authority?.TrimEnd('/');
                        if (!string.IsNullOrEmpty(authority))
                        {
                            context.ProtocolMessage.IssuerAddress = authority + "/connect/logout";
                        }
                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = ".AspNetCore.Cookies";
                options.Cookie.Path = "/";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(12);
                options.Cookie.MaxAge = TimeSpan.FromDays(1);
                options.LoginPath = "/authentication/login";
                options.LogoutPath = "/authentication/logout";
            });

        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
            .SetApplicationName("MotivationHRWeb")
            .AddKeyManagementOptions(options =>
            {
                options.NewKeyLifetime = TimeSpan.FromDays(90);
                options.AutoGenerateKeys = true;
            });

        builder.Services.ConfigureCookieOidcRefresh(CookieAuthenticationDefaults.AuthenticationScheme, AppData.OidcSchemeName);
    }
}
