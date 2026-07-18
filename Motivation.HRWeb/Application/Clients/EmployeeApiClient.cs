using Calabonga.OperationResults;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients.Base;

namespace Motivation.HRWeb.Application.Clients;

public class EmployeeApiClient : BaseApiClient
{
    public EmployeeApiClient(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
        : base(httpClientFactory, httpContextAccessor, configuration, "HRService"){}

    public async Task<Operation<IEnumerable<EmployeeDto>?, string>> GetAllAsync(CancellationToken cancellationToken)
    {
        var employees = await SendAsync<IEnumerable<EmployeeDto>>(HttpMethod.Get, "/api/employees/all", null, cancellationToken);
        if (!employees.Ok)
            return Operation.Error(employees.Error);
        return employees;
    }

    public async Task<Operation<bool, string>> CreateAsync(EmployeeCreateDto dto, CancellationToken cancellationToken)
    {
        var createResult = await SendAsync<bool>(HttpMethod.Post, "/api/employees/create", dto, cancellationToken);
        if (!createResult.Ok)
            return Operation.Error(createResult.Error);
        return createResult;
    }

    public async Task<Operation<bool, string>> UpdateAsync(EmployeeUpdateDto dto, CancellationToken cancellationToken)
    {
        var updateResult = await SendAsync<bool>(HttpMethod.Put, $"/api/employees/{dto.Code}/update", dto, cancellationToken);
        if (!updateResult.Ok)
            return Operation.Error(updateResult.Error);
        return updateResult;
    }

    public async Task<Operation<bool, string>> DeleteAsync(string code, CancellationToken cancellationToken)
    {
        var deleteResult = await SendAsync<bool>(HttpMethod.Delete, $"/api/employees/{code}", null, cancellationToken);
        if (!deleteResult.Ok)
            return Operation.Error(deleteResult.Error);
        return deleteResult;
    }

    public async Task<Operation<bool, string>> AssignPositionToEmployeeAsync(string employeeCode, AssignEmployeeToPositionDto model, CancellationToken cancellationToken)
    {
        var result = await SendAsync<bool>(HttpMethod.Put, $"/api/employees/{employeeCode}/assign", model, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }
    public async Task<Operation<bool, string>> DismissEmployeeFromPositionAsync(string employeeCode, string positionCode, CancellationToken cancellationToken)
    {
        var result = await SendAsync<bool>(HttpMethod.Delete, $"/api/employees/{employeeCode}/dismiss/{positionCode}", null, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    public async Task<Operation<bool, string>> SetActualHoursAsync(string employeeCode, SetActualHoursDto model, CancellationToken cancellationToken)
    {
        var result = await SendAsync<bool>(HttpMethod.Put, $"/api/employees/{employeeCode}/set-actual-hours", model, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    public async Task<Operation<bool, string>> RemoveActualHoursAsync(string employeeCode, RemoveActualHoursDto model, CancellationToken cancellationToken)
    {
        var result = await SendAsync<bool>(HttpMethod.Delete, $"/api/employees/{employeeCode}/remove-actual-hours", model, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    public async Task<Operation<DeductionDto, string>> SetDeductionAsync(string employeeCode, SetDeductionDto model, CancellationToken cancellationToken)
    {
        var result = await SendAsync<DeductionDto>(HttpMethod.Put, $"/api/employees/{employeeCode}/set-deduction", model, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    internal async Task<Operation<bool, string>> RemoveDeductionAsync(string employeeCode, RemoveDeductionDto removeDto, CancellationToken cancellationToken)
    {
        var result = await SendAsync<bool>(HttpMethod.Delete, $"/api/employees/{employeeCode}/remove-deduction", removeDto, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return true;
    }

    internal async Task<Operation<bool, string>> RemoveExtraPartAsync(string employeeCode, RemoveExtraPartDto removeModel, CancellationToken cancellationToken)
    {
        var result = await SendAsync<bool>(HttpMethod.Delete, $"/api/employees/{employeeCode}/remove-extra-part", removeModel, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    }

    public async Task<Operation<ExtraPartDto, string>> SetExtraPartAsync(string employeeCode, CreateExtraPartDto model, CancellationToken cancellationToken)
    {
        var result = await SendAsync<ExtraPartDto>(HttpMethod.Put, $"/api/employees/{employeeCode}/set-extra-part", model, cancellationToken);
        if (!result.Ok)
            return Operation.Error(result.Error);
        return result;
    } 
}