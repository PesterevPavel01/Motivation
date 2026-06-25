using Calabonga.OperationResults;
using Microsoft.AspNetCore.Components;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients.Base;

namespace Motivation.HRWeb.Application.Clients;

public class PositionApiClient : BaseApiClient
{
    public PositionApiClient(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
        : base(httpClientFactory, httpContextAccessor, configuration, "HRService")
    {
    }

    public async Task<Operation<IEnumerable<PositionDto>?, string>> GetAllAsync(CancellationToken cancellationToken)
    {
        var positions = await SendAsync<IEnumerable<PositionDto>>(HttpMethod.Get, "/api/positions/all", null, cancellationToken);
        if (!positions.Ok)
            return Operation.Error(positions.Error);
        return positions;
    }


    public async Task<Operation<bool, string>> CreateAsync(PositionCreateDto dto, CancellationToken cancellationToken)
    {
        var result = await SendAsync<bool>(HttpMethod.Post, "/api/positions/create", dto, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    public async Task<Operation<bool, string>> UpdateAsync(PositionUpdateDto dto, CancellationToken cancellationToken)
    {
        var updateResult = await SendAsync<bool>(HttpMethod.Put, $"/api/positions/{dto.Code}/update", dto, cancellationToken);
        if (!updateResult.Ok)
            return Operation.Error(updateResult.Error);
        return updateResult;
    }

    public async Task<Operation<bool, string>> DeleteAsync(string code, CancellationToken cancellationToken)
    {
        var deleteResult = await SendAsync<bool>(HttpMethod.Delete, $"/api/positions/{code}", null, cancellationToken);
        if (!deleteResult.Ok)
            return Operation.Error(deleteResult.Error);
        return deleteResult;
    }
}