using Calabonga.OperationResults;
using Mediator;
using Motivation.Domain.Enum;
using Motivation.HRWeb.Application.Mappings;
using Motivation.HRWeb.Application.Messages.KpiMessages;
using Motivation.HRWeb.Components.Pages.KpiPage.Models;
using Motivation.IdentityServer.Domain.Base;

namespace Motivation.HRWeb.Components.Pages.KpiPage.ViewModels;

public class EditKpiViewModel
{
    private readonly IMediator _mediator;

    public EditKpiViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }
    public string Title { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string CalculationType { get; set; } = string.Empty;
    public MeasurementUnitModel MeasurementUnit { get; set; } = null!;
    public string KpiType { get; set; } = string.Empty;

    public bool CustomerResponsibleFilter { get; set; }
    public bool SalesResponsibleFilter { get; set; }
    public bool ProductResponsibleFilter { get; set; }

    public List<KpiFilterItemModel> CustomerFilter { get; set; } = [];
    public List<KpiFilterItemModel> ProductFilter { get; set; } = [];
    public List<KpiFilterItemModel> CompanyFilter { get; set; } = [];

    public bool IsSaving { get; set; } = false;
    public string? ErrorMessage { get; set; }

    public void AddCustomerFilter() => CustomerFilter.Add(new KpiFilterItemModel());
    public void AddProductFilter() => ProductFilter.Add(new KpiFilterItemModel());
    public void AddCompanyFilter() => CompanyFilter.Add(new KpiFilterItemModel());

    public void RemoveCustomerFilter(int index) => CustomerFilter.RemoveAt(index);
    public void RemoveProductFilter(int index) => ProductFilter.RemoveAt(index);
    public void RemoveCompanyFilter(int index) => CompanyFilter.RemoveAt(index);

    public static EditKpiViewModel Create(IMediator mediator, KpiRowModel? kpi)
    {
        var viewModel = new EditKpiViewModel(mediator);
        if (kpi is not null)
        {
            viewModel.Title = kpi.Title;
            viewModel.Abbreviation = kpi.Abbreviation;
            viewModel.CalculationType = kpi.CalculationType;
            viewModel.MeasurementUnit = new MeasurementUnitModel() {Title = kpi.MeasurementUnit.Title, Code = kpi.MeasurementUnit.Code};
            viewModel.KpiType = kpi.KpiType;
            viewModel.CustomerFilter = kpi.CustomerFilter.Select(x => new KpiFilterItemModel { Title = x.Title, Include = x.Include }).ToList();
            viewModel.ProductFilter = kpi.ProductFilter.Select(x => new KpiFilterItemModel { Title = x.Title, Include = x.Include }).ToList();
            viewModel.CompanyFilter = kpi.CompanyFilter.Select(x => new KpiFilterItemModel { Title = x.Title, Include = x.Include }).ToList();
            viewModel.CustomerResponsibleFilter = kpi.CustomerResponsibleFilter;
            viewModel.SalesResponsibleFilter = kpi.SalesResponsibleFilter;
            viewModel.ProductResponsibleFilter = kpi.ProductResponsibleFilter;
        }
        else
        {
            viewModel.Title = string.Empty;
            viewModel.Abbreviation = string.Empty;
            viewModel.CalculationType = string.Empty;
            viewModel.MeasurementUnit = new ();
            viewModel.KpiType = string.Empty;
            viewModel.CustomerFilter.Clear();
            viewModel.ProductFilter.Clear();
            viewModel.CompanyFilter.Clear();
            viewModel.CustomerResponsibleFilter = false;
            viewModel.SalesResponsibleFilter = false;
            viewModel.ProductResponsibleFilter = false;
        }
        return viewModel;
    }

    public async Task<Operation<string?, string>> SaveAsync(string? code, bool isNew = false)
    {
        IsSaving = true;
        ErrorMessage = null;
        try
        {
            if (isNew)
            {
                var result = await _mediator.Send(new CreateKpi.Request(this.ToCreateDto()));
                if (!result.Ok)
                {
                    ErrorMessage = result.Error;
                    return Operation.Error(result.Error);
                }
                return result.Result.Code;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    ErrorMessage = "Code is null or white space!";
                    return Operation.Error("Code is null or white space!");
                }
                var result = await _mediator.Send(new UpdateKpi.Request(this.ToUpdateDto(code)));
                if (!result.Ok)
                {
                    ErrorMessage = result.Error;
                    return Operation.Error(result.Error);
                }
                return code;
            }
        }
        catch (Exception exception) 
        {
            ErrorMessage = exception.Message;
            return Operation.Error(exception.Message);
        }
        finally
        {
            IsSaving = false;
        }
    }

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Title)) { ErrorMessage = "Поле 'Наименование' обязательное."; return false; }
        if (string.IsNullOrWhiteSpace(Abbreviation)) { ErrorMessage = "Поле 'Краткое наименование' обязательное."; return false; }
        if (string.IsNullOrWhiteSpace(CalculationType)) { ErrorMessage = "Поле 'Порядок расчета' обязательное."; return false; }
        if (string.IsNullOrWhiteSpace(MeasurementUnit.Code)) { ErrorMessage = "Поле 'Единица измерения' обязательное."; return false; }
        if (string.IsNullOrWhiteSpace(KpiType)) { ErrorMessage = "Поле 'Тип' обязательное."; return false; }
        
        return true;
    }
}