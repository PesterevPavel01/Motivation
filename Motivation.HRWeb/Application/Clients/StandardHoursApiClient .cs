using Calabonga.OperationResults;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients.Base;

namespace Motivation.HRWeb.Application.Clients;

public class StandardHoursApiClient : BaseApiClient
{
    public StandardHoursApiClient(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
        : base(httpClientFactory, httpContextAccessor, configuration, "HRService")
    {
    }

    public async Task<Operation<StandardHoursDto?, string>> GetByMonthAsync(string workWeekType, int month, int year, CancellationToken cancellationToken)
    {
        var result = await SendAsync<StandardHoursDto>(HttpMethod.Get, $"/api/standard-hours/by-month/{month}/{year}/{workWeekType}", null, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    public async Task<Operation<StandardHoursDto?, string>> CreateAsync(StandardHoursCreateDto dto, CancellationToken cancellationToken)
    {
        var result = await SendAsync<StandardHoursDto>(HttpMethod.Post, "/api/standard-hours/create", dto, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    public async Task<Operation<bool, string>> UpdateAsync(Guid id, StandardHoursUpdateDto dto, CancellationToken cancellationTokent)
    {
        var result = await SendAsync<bool>(HttpMethod.Put, $"/api/standard-hours/{id}/update", dto, cancellationTokent);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }
}
