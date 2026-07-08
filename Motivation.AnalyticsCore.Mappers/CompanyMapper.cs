using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Mappers;

public static class CompanyMapper
{
    public static Operation<Company, string> ToEntity(CompanyDto dto)
    {
        if (dto is null)
            return Operation.Error("Company sync DTO is null.");

        var titleResult = TitleValue.Create(dto.Title);
        if (!titleResult.Ok)
            return Operation.Error(titleResult.Error!);

        var codeResult = CodeValue.Create(dto.Code);
        if (!codeResult.Ok)
            return Operation.Error(codeResult.Error!);

        var companyResult = Company.Create(titleResult.Result, codeResult.Result);
        if (!companyResult.Ok)
            return Operation.Error(companyResult.Error!);

        var company = companyResult.Result!;

        return company;
    }

    public static Operation<Company, string> EditEntity(this Company company, CompanyDto model)
    {
        if (model is null)
            return Operation.Error("Customer DTO is null.");

        var titleResult = TitleValue.Create(model.Title);
        if (!titleResult.Ok)
            return Operation.Error(titleResult.Error!);

        return company.EditEntity(
            titleResult.Result
        );
    }
}
