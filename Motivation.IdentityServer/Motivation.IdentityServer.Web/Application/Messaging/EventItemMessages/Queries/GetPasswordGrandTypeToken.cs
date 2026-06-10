using Calabonga.OperationResults;
using Mediator;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Motivation.IdentityServer.Infrastructure;
using Motivation.IdentityServer.Web.Application.Services;
using OpenIddict.Abstractions;
using System.Security.Claims;

namespace Motivation.IdentityServer.Web.Application.Messaging.EventItemMessages.Queries;

public class GetPasswordGrandTypeToken
{
    public record Request(HttpContext HttpContext) : IRequest<Operation<ClaimsPrincipal, string>>;

    public class Handler(IOpenIddictApplicationManager applicationManager, 
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAccountService accountService)
        : IRequestHandler<Request, Operation<ClaimsPrincipal, string>>
    {
        public async ValueTask<Operation<ClaimsPrincipal, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var httpContext = request.HttpContext;
            // Получаем OIDC запрос из контекста
            var oidcRequest = httpContext.GetOpenIddictServerRequest();
            if (oidcRequest == null || !oidcRequest.IsPasswordGrantType())
            {
                return Operation.Error("Invalid grant type");
            }

            var username = oidcRequest.Username;
            var password = oidcRequest.Password;

            if (string.IsNullOrEmpty(username))
            {
                return Operation.Error("Username is required");
            }

            // Находим пользователя
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Operation.Error("Invalid username or password");
            }

            // Проверяем, может ли пользователь войти
            if (!await signInManager.CanSignInAsync(user))
            {
                return Operation.Error("User cannot sign in");
            }

            // Проверяем блокировку
            if (userManager.SupportsUserLockout && await userManager.IsLockedOutAsync(user))
            {
                return Operation.Error("User is locked out");
            }

            // Проверяем пароль
            if (!await userManager.CheckPasswordAsync(user, password))
            {
                if (userManager.SupportsUserLockout)
                {
                    await userManager.AccessFailedAsync(user);
                }
                return Operation.Error("Invalid username or password");
            }

            // Сбрасываем счётчик неудачных попыток
            if (userManager.SupportsUserLockout)
            {
                await userManager.ResetAccessFailedCountAsync(user);
            }

            // Получаем principal для пользователя
            var principal = await accountService.GetPrincipalForUserAsync(user);

            return principal;
        }
    }
}
