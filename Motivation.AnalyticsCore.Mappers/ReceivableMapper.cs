using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Mappers;

public static class ReceivableMapper
{
    public static Operation<Receivable, string> ToEntity(ReceivableDto dto, Company company, Customer customer)
    {
        if (dto is null)
            return Operation.Error("Receivable DTO is null.");
        if (company is null)
            return Operation.Error("Company is null.");
        if (customer is null)
            return Operation.Error("Customer is null.");

        var amountResult = AmountValue.Create(dto.Amount);
        if (!amountResult.Ok)
            return Operation.Error(amountResult.Error);

        return Receivable.Create(
            amountResult.Result,
            customer,
            company,
            dto.AsOfDate
        );
    }

    public static ReceivableDto ToDto(this Receivable entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var customerDto = entity.Customer.ToDto();

        var companyDto = new CompanyDto
        {
            Code = entity.Company?.Code?.Value ?? string.Empty,
            Title = entity.Company?.Title?.Value ?? string.Empty
        };

        var receivableDto = new ReceivableDto
        {
            Amount = entity.Amount?.Value ?? 0,
            Customer = customerDto,
            AsOfDate = entity.AsOfDate,
            Company = companyDto
        };

        return receivableDto;
    }
}
