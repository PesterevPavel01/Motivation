using Calabonga.OperationResults;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.Enum;
using Motivation.HRWeb.Application.Clients.Base;

namespace Motivation.HRWeb.Application.Clients;

public class KpiApiClient : BaseApiClient
{
    public KpiApiClient(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
        : base(httpClientFactory, httpContextAccessor, configuration, "KpiService")
    {}

    public async Task<Operation<IEnumerable<KpiDto>?, string>> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await SendAsync<IEnumerable<KpiDto>>(HttpMethod.Get, "/api/kpi/all", null, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    public async Task<Operation<EmployeeDto?, string>> GetEmployeeAsync(string employeeCode, CancellationToken cancellationToken)
    {
        var result = await SendAsync<EmployeeDto>(HttpMethod.Get, $"/api/employee/{employeeCode}/kpis", null, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    public async Task<Operation<IEnumerable<string>?, string>> GetAllTypesAsync(CancellationToken cancellationToken)
    {
        var result = await SendAsync<IEnumerable<string>>(HttpMethod.Get, "/api/kpi/types", null, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    internal async Task<Operation<IEnumerable<string>?, string>> GetAllCalculationTypesAsync(CancellationToken cancellationToken)
    {
        var result = await SendAsync<IEnumerable<string>>(HttpMethod.Get, "/api/kpi/calculationType/all", null, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    internal async Task<Operation<bool, string>> DeleteAsync(string code, CancellationToken cancellationToken)
    {
        var result = await SendAsync<bool>(HttpMethod.Delete, $"/api/kpi/{code}", null, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    internal async Task<Operation<bool, string>> UpdateAsync(KpiUpdateDto model, CancellationToken cancellationToken)
    {
        var result = await SendAsync<bool>(HttpMethod.Put, $"/api/kpi/{model.Code}/update", model, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    public async Task<Operation<bool, string>> SetKpiTargetAsync(SetEmployeeKpiTarget model, CancellationToken cancellationToken)
    {
        var result = await SendAsync<bool>(HttpMethod.Put, $"/api/employee/kpi/set-target", model, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    internal async Task<Operation<KpiDto?, string>> CreateAsync(KpiCreateDto model, CancellationToken cancellationToken)
    {
        var result = await SendAsync<KpiDto>(HttpMethod.Post, $"/api/kpi/create", model, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    internal async Task<Operation<bool, string>> CreatePositionKpiAsync(String positionCode, IEnumerable<AssignKpiToPositionDto> models, CancellationToken cancellationToken)
    {
        var result = await SendAsync<bool>(HttpMethod.Put, $"/api/positions/{positionCode}/assign-kpis", models, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }
}
