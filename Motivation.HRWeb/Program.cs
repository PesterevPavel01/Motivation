using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Motivation.HRWeb;
using Motivation.HRWeb.Application.Interfaces;
using Motivation.HRWeb.Application.Services;
using Motivation.HRWeb.Components;
using Motivation.HRWeb.Extensions;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDataProtection()
//    .UseEphemeralDataProtectionProvider()
//    .SetApplicationName("MotivationHRWeb");

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
    .SetApplicationName("MotivationHRWeb")
    .AddKeyManagementOptions(options =>
    {
        options.NewKeyLifetime = TimeSpan.FromDays(90);
        options.AutoGenerateKeys = true;
    });

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
        options.RequireHttpsMetadata = false; // Важно!
        options.SaveTokens = true;
        //options.SaveTokens = false;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.GetClaimsFromUserInfoEndpoint = false;
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
            OnTicketReceived = context =>
            {
                if (context is null || context.Properties is null || context.Properties.Items is null)
                {
                    return Task.CompletedTask;
                }

                var properties = new AuthenticationProperties(context.Properties.Items);
                properties.StoreTokens(context.Properties.GetTokens().Where(token => token switch
                {
                    { 
                        Name: "access_token" or "refresh_token"
                    } => true,
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
                // Логируем параметры, которые отправляются в Identity Server
                Console.WriteLine("=== OnRedirectToIdentityProvider ===");
                Console.WriteLine($"ProtocolMessage: {context.ProtocolMessage}");
                Console.WriteLine($"RedirectUri: {context.ProtocolMessage.RedirectUri}");
                Console.WriteLine($"ClientId: {context.ProtocolMessage.ClientId}");
                Console.WriteLine($"ResponseType: {context.ProtocolMessage.ResponseType}");
                Console.WriteLine($"ResponseMode: {context.ProtocolMessage.ResponseMode}");
                Console.WriteLine($"Scope: {context.ProtocolMessage.Scope}");
                Console.WriteLine($"State: {context.ProtocolMessage.State}");
                Console.WriteLine($"Nonce: {context.ProtocolMessage.Nonce}");

                // Получаем параметры через Parameters
                if (context.ProtocolMessage.Parameters.TryGetValue("code_challenge", out var codeChallenge))
                {
                    Console.WriteLine($"CodeChallenge: {codeChallenge}");
                }
                if (context.ProtocolMessage.Parameters.TryGetValue("code_challenge_method", out var codeChallengeMethod))
                {
                    Console.WriteLine($"CodeChallengeMethod: {codeChallengeMethod}");
                }

                Console.WriteLine($"=== Before forming IssuerAddress ===");
                Console.WriteLine($"options.Authority: {context.Options.Authority}");
                Console.WriteLine($"options.MetadataAddress: {context.Options.MetadataAddress}");
                Console.WriteLine("=====================================");
                Console.WriteLine($"IssuerAddress: {context.ProtocolMessage.IssuerAddress}");
                Console.WriteLine("=====================================");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                Console.WriteLine("=== OnMessageReceived ===");
                Console.WriteLine($"Response Status Code: {context.Response?.StatusCode}");
                Console.WriteLine($"ProtocolMessage: {context.ProtocolMessage}");
                Console.WriteLine($"Issuer: {context.ProtocolMessage?.IssuerAddress}");
                Console.WriteLine($"AuthorizationEndpoint: {context.ProtocolMessage?.GetParameter("authorization_endpoint")}");
                Console.WriteLine($"TokenEndpoint: {context.ProtocolMessage?.GetParameter("token_endpoint")}");
                Console.WriteLine("=========================");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"=== OnAuthenticationFailed ===");
                Console.WriteLine($"Exception: {context.Exception.Message}");
                Console.WriteLine($"ProtocolMessage: {context.ProtocolMessage}");
                Console.WriteLine("=================================");
                return Task.CompletedTask;
            },
            OnRedirectToIdentityProviderForSignOut = context =>
            {
                var authority = context.Options.Authority?.TrimEnd('/');
                if (!string.IsNullOrEmpty(authority))
                {
                    context.ProtocolMessage.IssuerAddress = authority + "/connect/logout";
                }
                Console.WriteLine("=== OnRedirectToIdentityProviderForSignOut ===");
                Console.WriteLine($"IssuerAddress: {context.ProtocolMessage.IssuerAddress}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                try
                {
                    Console.WriteLine($"=== OnTokenValidated ===");
                    Console.WriteLine($"Principal: {context.Principal?.Identity?.Name}");
                    // Можно добавить логирование claims
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in OnTokenValidated: {ex.Message}");
                    Console.WriteLine($"Stack: {ex.StackTrace}");
                    throw; // или return Task.FromException(ex);
                }
                return Task.CompletedTask;
            },
            OnRemoteFailure = context =>
            {
                Console.WriteLine("=== OnRemoteFailure ===");
                Console.WriteLine($"Failure: {context.Failure?.Message}");
                Console.WriteLine("=========================");
                return Task.CompletedTask;
            }
        };
        Console.WriteLine($"Authority after bind: {options.Authority}");
        Console.WriteLine($"MetadataAddress: {options.MetadataAddress}");
        Console.WriteLine($"CallbackPath: {options.CallbackPath}");
        Console.WriteLine($"ResponseMode: {options.ResponseMode}");
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = ".AspNetCore.Cookies"; // или своё имя
        options.Cookie.Path = "/";
        //options.Cookie.Domain = "catalog.events.fvds.ru";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.LoginPath = "/authentication/login";
        options.LogoutPath = "/authentication/logout";
    });

builder.Services.ConfigureCookieOidcRefresh(CookieAuthenticationDefaults.AuthenticationScheme, AppData.OidcSchemeName);

builder.Services.AddAuthorization();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddHttpClient();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();

var config = builder.Configuration.GetSection("OpenIDConnectSettings");
var responseMode = config["ResponseMode"];
Console.WriteLine($"ResponseMode from config: {responseMode}");
var authority = builder.Configuration["OpenIDConnectSettings:Authority"];
Console.WriteLine($"Authority from config: {authority}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.Use((context, next) =>
    {
        context.Request.Scheme = "https";
        return next();
    });

    app.Use(async (context, next) =>
    {
        // Логируем только запросы к /signin-oidc
        if (context.Request.Path.StartsWithSegments("/signin-oidc"))
        {
            Console.WriteLine($"=== /signin-oidc request ===");
            Console.WriteLine($"Method: {context.Request.Method}");
            Console.WriteLine($"Scheme: {context.Request.Scheme}");
            Console.WriteLine($"Host: {context.Request.Host}");
            Console.WriteLine($"Path: {context.Request.Path}");
            Console.WriteLine($"Query: {context.Request.QueryString}");
            Console.WriteLine($"Headers: {string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
            Console.WriteLine("==============================");
        }
        await next();
    });

    app.Use(async (context, next) =>
    {
        await next();
        if (context.Response.StatusCode == 302 || context.Response.StatusCode == 301)
        {
            var location = context.Response.Headers["Location"].ToString();
            Console.WriteLine($"=== Redirect to: {location} ===");
        }
        var setCookie = context.Response.Headers["Set-Cookie"].ToString();
        if (!string.IsNullOrEmpty(setCookie))
        {
            Console.WriteLine($"=== Set-Cookie: {setCookie} ===");
        }
    });

    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/signin-oidc")
        {
            Console.WriteLine("=== Cookies in /signin-oidc request ===");
            foreach (var cookie in context.Request.Cookies)
            {
                Console.WriteLine($"{cookie.Key}: {cookie.Value}");
            }
        }
        await next();
    });

    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request to: {context.Request.Path}");
    await next();
});

app.UseRouting();

app.Use(async (context, next) =>
{
    Console.WriteLine($"=== Before Authentication ===");
    Console.WriteLine($"Path: {context.Request.Path}");
    Console.WriteLine($"Method: {context.Request.Method}");
    await next();
});

app.UseAuthentication();

app.Use(async (context, next) =>
{
    var cookie = context.Request.Cookies[".AspNetCore.Cookies"];
    if (!string.IsNullOrEmpty(cookie))
    {
        Console.WriteLine($"Cookie found, length: {cookie.Length}");
    }
    else
    {
        Console.WriteLine("Cookie NOT found");
    }
    await next();
});

app.Use(async (context, next) =>
{
    Console.WriteLine($"=== After Authentication ===");
    Console.WriteLine($"Path: {context.Request.Path}");
    Console.WriteLine($"Method: {context.Request.Method}");
    await next();
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/.well-known/openid-configuration")
    {
        // Подменяем response, чтобы перехватить тело ответа
        var originalBody = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await next();

        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
        Console.WriteLine($"=== /.well-known/openid-configuration response ===");
        Console.WriteLine(responseBody);
        Console.WriteLine("===================================================");

        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBody);
        context.Response.Body = originalBody;
    }
    else
    {
        await next();
    }
});

app.UseAuthorization();    

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.MapGroup("/authentication").MapLoginAndLogout();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (CryptographicException ex)
    {
        Console.WriteLine($"=== Data Protection Error ===");
        Console.WriteLine($"Message: {ex.Message}");
        Console.WriteLine($"Stack: {ex.StackTrace}");
        throw; // или можно вернуть пользователю понятное сообщение
    }
});

app.Run();
