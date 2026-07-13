using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Mappers;

public static class CustomerGroupMapper
{
    public static Operation<CustomerGroup, string> ToEntity(CustomerGroupDto dto)
    {
        if (dto is null)
            return Operation.Error("CustomerGroup DTO is null.");

        var title = TitleValue.Create(dto.Title);
        if (!title.Ok)
            return Operation.Error(title.Error);

        var code = CodeValue.Create(dto.Code);
        if (!code.Ok)
            return Operation.Error(code.Error);

        CustomerGroup? parentGroup = null;
        if (dto.ParentGroup is not null)
        {
            var parentResult = ToEntity(dto.ParentGroup);
            if (!parentResult.Ok)
                return Operation.Error(parentResult.Error!);
            parentGroup = parentResult.Result;
        }

        return CustomerGroup.Create(title.Result, code.Result, parentGroup);
    }
    public static Operation<bool, string> EditEntity(this CustomerGroup customerGroup, CustomerGroupDto model)
    {
        if (model is null)
            return Operation.Error("Customer group DTO is null.");

        var titleResult = TitleValue.Create(model.Title);
        if (!titleResult.Ok)
            return Operation.Error(titleResult.Error!);

        return customerGroup.Edit(
            titleResult.Result,
            null
        );
    }
}
