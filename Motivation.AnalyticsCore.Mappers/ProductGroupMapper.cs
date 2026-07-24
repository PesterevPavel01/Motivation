using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Mappers;

public static class ProductGroupMapper
{
    public static ProductGroupDto ToDto(this ProductGroup source) 
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        return new ProductGroupDto()
        {
            Code = source.Code.Value,
            Title = source.Title.Value,
            ParentGroup = source.ParentGroup is null ?
                null
                : source.ParentGroup.ToDto(),
        };
    }

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

    public static Operation<bool, string> EditEntity(this ProductGroup productGroup, ProductGroupDto model)
    {
        if (model is null)
            return Operation.Error("Product group DTO is null.");

        var titleResult = TitleValue.Create(model.Title);
        if (!titleResult.Ok)
            return Operation.Error(titleResult.Error!);

        return productGroup.Edit(
            titleResult.Result,
            null
        );
    }
}
