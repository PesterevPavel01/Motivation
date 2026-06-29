using Motivation.Contracts.Dto;

namespace Motivation.HRWeb.Components.Pages.KpiPage.Models;

public class KpiRowModel
{
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string CalculationType { get; set; } = string.Empty;
    public MeasurementUnitModel MeasurementUnit { get; set; } = null!;
    public string KpiType { get; set; } = null!;

    public bool CustomerResponsibleFilter { get; set; }
    public bool SalesResponsibleFilter { get; set; }
    public bool ProductResponsibleFilter { get; set; }

    public bool HasCustomerFilter => CustomerFilter.Any();
    public bool HasProductFilter => ProductFilter.Any();
    public bool HasCompanyFilter => CompanyFilter.Any();

    public List<KpiFilterDto> CustomerFilter { get; set; } = [];
    public List<KpiFilterDto> ProductFilter { get; set; } = [];
    public List<KpiFilterDto> CompanyFilter { get; set; } = [];

    public bool IsEditing { get; set; } = false;
    public bool IsSaving { get; set; } = false;
}
