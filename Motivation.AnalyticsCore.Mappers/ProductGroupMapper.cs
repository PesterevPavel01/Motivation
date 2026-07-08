using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Mappers;

public static class ProductGroupMapper
{
    public static Operation<ProductGroup, string> ToEntity(ProductGroupDto dto)
    {
        if (dto is null)
            return Operation.Error("ProductGroup DTO is null.");

        var titleResult = TitleValue.Create(dto.Title);
        if (!titleResult.Ok)
            return Operation.Error(titleResult.Error);

        var codeResult = CodeValue.Create(dto.Code);
        if (!codeResult.Ok)
            return Operation.Error(codeResult.Error);

        ProductGroup? parentGroup = null;
        if (dto.ParentGroup is not null)
        {
            var parentResult = ToEntity(dto.ParentGroup);
            if (!parentResult.Ok)
                return Operation.Error(parentResult.Error);
            parentGroup = parentResult.Result;
        }

        return ProductGroup.Create(titleResult.Result, codeResult.Result, parentGroup);
    }
}
