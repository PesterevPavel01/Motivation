using Microsoft.AspNetCore.Authentication;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Motivation.HRWeb.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public EmployeeService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(CancellationToken cancellationToken = default)
    {
        var token = await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("Access token is missing.");

        var employeeServiceUrl = _configuration["Services:HRService"]
              ?? throw new InvalidOperationException("HRService URL is not configured.");

        var requestUri = $"{employeeServiceUrl.TrimEnd('/')}/api/employees/all";

        using var client = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var employees = JsonSerializer.Deserialize<IEnumerable<EmployeeDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return employees ?? Enumerable.Empty<EmployeeDto>();
    }
}
