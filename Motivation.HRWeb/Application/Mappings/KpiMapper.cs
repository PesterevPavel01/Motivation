using Motivation.Contracts.Dto;
using Motivation.Domain.Enum;
using Motivation.HRWeb.Components.Pages.KpiPage.Models;
using Motivation.HRWeb.Components.Pages.KpiPage.ViewModels;

namespace Motivation.HRWeb.Application.Mappings;

public static class KpiMapper
{
    public static KpiUpdateDto ToUpdateDto(this EditKpiViewModel model, string code)
    {
        return new KpiUpdateDto() {
            Code = code,
            Title = model.Title,
            KpiType = model.KpiType,
            Abbreviation = model.Abbreviation,
            CalculationType = model.CalculationType,
            MeasurementUnitCode = model.MeasurementUnit.Code,
            CustomerFilter = model.CustomerFilter.Select(x => new KpiFilterDto(x.Title, x.Include, KpiFilterType.Customer)).ToList(),
            SupplierFilter = model.SupplierFilter.Select(x => new KpiFilterDto(x.Title, x.Include, KpiFilterType.Supplier)).ToList(),
            ProductFilter = model.ProductFilter.Select(x => new KpiFilterDto(x.Title, x.Include, KpiFilterType.Product)).ToList(),
            ProductStatusFilter = model.ProductStatusFilter.Select(x => new KpiFilterDto(x.Title, x.Include, KpiFilterType.ProductStatus)).ToList(),
            CompanyFilter = model.CompanyFilter.Select(x => new KpiFilterDto(x.Title, x.Include, KpiFilterType.Company)).ToList(),
            CustomerResponsibleFilter = model.CustomerResponsibleFilter ? [new KpiFilterDto("default", default, KpiFilterType.CustomerResponsible) ]: [],
            ProductResponsibleFilter = model.ProductResponsibleFilter ? [new KpiFilterDto("default", default, KpiFilterType.ProductResponsible)] : [],
            SalesResponsibleFilter = model.SalesResponsibleFilter ? [new KpiFilterDto("default", default, KpiFilterType.SalesResponsible)] : []
        };
    }

    public static KpiCreateDto ToCreateDto(this EditKpiViewModel model)
    {
        return new KpiCreateDto()
        {
            Title = model.Title,
            Abbreviation = model.Abbreviation,
            KpiType = model.KpiType,
            CalculationType = model.CalculationType,
            MeasurementUnitCode = model.MeasurementUnit.Code,
            CustomerFilter = model.CustomerFilter.Select(x => new KpiFilterDto(x.Title, x.Include, KpiFilterType.Customer)).ToList(),
            SupplierFilter = model.SupplierFilter.Select(x => new KpiFilterDto(x.Title, x.Include, KpiFilterType.Supplier)).ToList(),
            ProductFilter = model.ProductFilter.Select(x => new KpiFilterDto(x.Title, x.Include, KpiFilterType.Product)).ToList(),
            ProductStatusFilter = model.ProductStatusFilter.Select(x => new KpiFilterDto(x.Title, x.Include, KpiFilterType.ProductStatus)).ToList(),
            CompanyFilter = model.CompanyFilter.Select(x => new KpiFilterDto(x.Title, x.Include, KpiFilterType.Company)).ToList(),
            CustomerResponsibleFilter = model.CustomerResponsibleFilter ? [new KpiFilterDto("default", default, KpiFilterType.CustomerResponsible)] : [],
            ProductResponsibleFilter = model.ProductResponsibleFilter ? [new KpiFilterDto("default", default, KpiFilterType.ProductResponsible)] : [],
            SalesResponsibleFilter = model.SalesResponsibleFilter ? [new KpiFilterDto("default", default, KpiFilterType.SalesResponsible)] : []
        };
    }
}
