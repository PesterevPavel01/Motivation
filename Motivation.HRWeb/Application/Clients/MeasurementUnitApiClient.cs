using Calabonga.OperationResults;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients.Base;

namespace Motivation.HRWeb.Application.Clients;

public class MeasurementUnitApiClient : BaseApiClient
{
    public MeasurementUnitApiClient(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
        : base(httpClientFactory, httpContextAccessor, configuration, "KpiService")
    {
    }

    public async Task<Operation<List<MeasurementUnitDto>?, string>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await SendAsync<List<MeasurementUnitDto>>(HttpMethod.Get, "/api/kpi/unit/all", null, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result.Result;
    }
}
