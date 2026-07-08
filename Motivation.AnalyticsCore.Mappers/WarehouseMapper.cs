using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Mappers;

public static class WarehouseMapper
{
    public static Operation<Warehouse, string> ToEntity(WarehouseDto dto)
    {
        if (dto is null)
            return Operation.Error("Warehouse DTO is null.");

        var titleResult = TitleValue.Create(dto.Title);
        if (!titleResult.Ok)
            return Operation.Error(titleResult.Error);

        var codeResult = CodeValue.Create(dto.Code);
        if (!codeResult.Ok)
            return Operation.Error(codeResult.Error);

        return Warehouse.Create(titleResult.Result, codeResult.Result);
    }
}