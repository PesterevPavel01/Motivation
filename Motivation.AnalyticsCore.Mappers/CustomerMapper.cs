using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Mappers;

public static class CustomerMapper
{
    public static Operation<Customer, string> ToEntity(CustomerDto dto, Dictionary<string, CustomerGroup> existingCustomerGroups)
    {
        if (dto is null)
            return Operation.Error("Customer DTO is null.");

        var title = TitleValue.Create(dto.Title);
        if (!title.Ok)
            return Operation.Error(title.Error);

        var code = CodeValue.Create(dto.Code);
        if (!code.Ok)
            return Operation.Error(code.Error);

        var fullName = string.IsNullOrWhiteSpace(dto.FullName) ? FullNameValue.Create(dto.Title) : FullNameValue.Create(dto.FullName);
        if (!fullName.Ok)
            return Operation.Error(fullName.Error);

        ManagerValue? manager = null;
        if (!string.IsNullOrWhiteSpace(dto.Manager))
        {
            var managerResult = ManagerValue.Create(dto.Manager);
            if (!fullName.Ok)
                return Operation.Error(fullName.Error);
            manager = managerResult.Result;
        }

        var customerGroup = GetCustomerGroup(dto.CustomerGroup, existingCustomerGroups);

        return Customer.Create(
            title.Result,
            code.Result,
            fullName.Result,
            manager,
            customerGroup.Result);
    }

    public static Operation<Customer, string> EditEntity(this Customer customer, CustomerDto dto, Dictionary<string, CustomerGroup> existingCustomerGroups)
    {
        if (dto is null)
            return Operation.Error("Customer DTO is null.");

        var titleResult = TitleValue.Create(dto.Title);
        if (!titleResult.Ok)
            return Operation.Error(titleResult.Error!);

        var codeResult = CodeValue.Create(dto.Code);
        if (!codeResult.Ok)
            return Operation.Error(codeResult.Error);

        ManagerValue? manager = null;
        if (!string.IsNullOrWhiteSpace(dto.Manager))
        {
            var managerResult = ManagerValue.Create(dto.Manager);
            if (!managerResult.Ok)
                return Operation.Error(managerResult.Error);
            manager = managerResult.Result;
        }

        FullNameValue? fullName = null;
        if (!string.IsNullOrWhiteSpace(dto.FullName))
        {
            var statusResult = FullNameValue.Create(dto.FullName);
            if (!statusResult.Ok)
                return Operation.Error(statusResult.Error);
            fullName = statusResult.Result;
        }

        var customerGroup = GetCustomerGroup(dto.CustomerGroup, existingCustomerGroups);

        return customer.EditEntity(
            titleResult.Result,
            fullName,
            manager,
            customerGroup.Result
        );
    }

    private static Operation<CustomerGroup?, string> GetCustomerGroup(CustomerGroupDto? model, Dictionary<string, CustomerGroup> existingCustomerGroups)
    {
        if (model is null)
            return null;

        var customerGroupCode = CodeValue.Create(model.Code);
        if (!customerGroupCode.Ok)
            return Operation.Error(customerGroupCode.Error);

        var code = customerGroupCode.Result.Value;
        if (existingCustomerGroups.TryGetValue(code, out var existingCustomerGroup))
            return existingCustomerGroup;

        var customerGroupResult = CustomerGroupMapper.ToEntity(model);
        if (!customerGroupResult.Ok)
            return Operation.Error(customerGroupResult.Error);

        existingCustomerGroups[code] = customerGroupResult.Result;

        return customerGroupResult.Result;
    }
}
