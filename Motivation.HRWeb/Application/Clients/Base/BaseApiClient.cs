using Calabonga.OperationResults;
using Microsoft.AspNetCore.Authentication;
using Motivation.HRWeb.Exceptions;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Motivation.HRWeb.Application.Clients.Base;

public abstract class BaseApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly string _serviceName;

    protected BaseApiClient(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        string serviceName)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _serviceName = serviceName;
    }

    protected async Task<Operation<T?, string>> SendAsync<T>(
        HttpMethod method,
        string endpointPath,
        object? body = null,
        CancellationToken cancellationToken = default)
    {
     
        var token = await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");
        if (string.IsNullOrWhiteSpace(token))
            return Operation.Error("Access token is missing.");

        var baseUrl = _configuration[$"Services:{_serviceName}"];
        if (string.IsNullOrWhiteSpace(baseUrl))
            return Operation.Error($"Service {_serviceName} URL is not configured.");

        var requestUri = $"{baseUrl.TrimEnd('/')}/{endpointPath.TrimStart('/')}";

        using var client = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(method, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (body != null)
        {
            var json = JsonSerializer.Serialize(body);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        }

        using var response = await client.SendAsync(request, cancellationToken);

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshToken = await _httpContextAccessor.HttpContext!.GetTokenAsync("refresh_token");
                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    throw new RedirectToLoginException();
                }
                return Operation.Error($"Ошибка авторизации. ПЕРЕЗАГРУЗИТЕ СТРАНИЦУ!");
            }
            return Operation.Error(responseJson);
        }

        if (string.IsNullOrWhiteSpace(responseJson))
            return default;

        return JsonSerializer.Deserialize<T?>(responseJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}