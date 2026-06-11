using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Motivation.IdentityServer.Domain.Base;
using Motivation.KpiService.Definitions.OpenIddict;
using System.Text.Json;

namespace Motivation.KpiService.Definitions.Authorizations
{
    /// <summary>
    /// Authorization Policy registration
    /// </summary>
    public class AuthorizationDefinition : AppDefinition
    {
        /// <summary>
        /// Configure services for current microservice
        /// </summary>
        /// <param name="builder"></param>
        public override void ConfigureServices(WebApplicationBuilder builder)
        {
            var url = builder.Configuration.GetSection("AuthServer").GetValue<string>("Url");

            builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, "Bearer", options =>
            {
                options.SaveToken = true;
                options.Audience = AppData.AudienceApi;
                options.Authority = url;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuer = url,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        // Ensure we always have an error and error description.
                        if (string.IsNullOrEmpty(context.Error))
                        {
                            context.Error = "invalid_token";
                        }

                        if (string.IsNullOrEmpty(context.ErrorDescription))
                        {
                            context.ErrorDescription = "This request requires a valid JWT access token to be provided";
                        }

                        // Add some extra context for expired tokens.
                        if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            var authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
                            context.Response.Headers.Append("x-token-expired", authenticationException?.Expires.ToString("o"));
                            context.ErrorDescription = $"The token expired on {authenticationException?.Expires:o}";
                        }

                        return context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            error = context.Error,
                            error_description = context.ErrorDescription
                        }));
                    }
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(AppData.PolicyDefaultName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                });

                options.AddPolicy(AppData.PolicySyncAccess, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(context =>
                    {
                        var scopeClaim = context.User.FindFirst("scope")?.Value;
                        var hasSyncScope = scopeClaim?.Contains(AppData.ScopeSync) == true;

                        var roles = context.User.FindAll("role").Select(c => c.Value);
                        var isAdmin = roles.Contains(AppData.SystemAdministratorRoleName);

                        return hasSyncScope || isAdmin;
                    });
                });

                options.AddPolicy("Administrator", policy =>
                    policy.RequireRole("Administrator"));
                options.AddPolicy("Constructor", policy =>
                    policy.RequireRole("Administrator", "constructor"));
            });

            builder.Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            builder.Services.AddSingleton<IAuthorizationHandler, AppPermissionHandler>();
        }

        /// <summary>
        /// Configure application for current microservice
        /// </summary>
        /// <param name="app"></param>
        public override void ConfigureApplication(WebApplication app)
        {
            app.UseRouting();
            app.UseCors(AppData.PolicyCorsName);
            app.UseAuthentication();
            app.UseAuthorization();

            // registering UserIdentity helper as singleton
            UserIdentity.Instance.Configure(app.Services.GetService<IHttpContextAccessor>()!);
        }
    }
}
