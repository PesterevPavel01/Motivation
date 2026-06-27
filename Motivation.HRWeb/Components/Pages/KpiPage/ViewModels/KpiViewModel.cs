using Calabonga.OperationResults;
using Mediator;
using Motivation.HRWeb.Application.Messages.KpiMessages;
using Motivation.HRWeb.Application.Messages.UnitMessages;
using Motivation.HRWeb.Components.Pages.KpiPage.Models;
using System.IO.Pipelines;

namespace Motivation.HRWeb.Components.Pages.KpiPage.ViewModels;

public class KpiViewModel
{
    private readonly IMediator _mediator;

    public KpiViewModel(IMediator mediator) => _mediator = mediator;

    public List<KpiRowModel> Kpis { get; private set; } = [];
    public List<MeasurementUnitModel> Units { get; private set; } = [];
    public List<KpiTypesModel> KpiTypes { get; private set; } = [];
    public List<CalculationTypeModel> CalculationTypes { get; private set; } = [];

    public string? ErrorMessage { get; private set; }
    public bool IsLoading { get; private set; }

    public bool IsModalOpen { get; private set; }
    public bool IsCreateModalOpen { get; private set; }
    public bool IsDeleteModalOpen { get; private set; }
    public KpiRowModel? SelectedKpi { get; private set; }

    public async Task<Operation<bool, string>> LoadAsync(CancellationToken cancellationToken = default)
    {
        IsLoading = true;
        ErrorMessage = null;
        Units.Clear();
        KpiTypes.Clear();

        try
        {
            var loadResult = await LoadKpiAsync(cancellationToken);
            if (!loadResult.Ok)
            {
                return Operation.Error(loadResult.Error);
            }

            loadResult = await LoadUnitsAsync(cancellationToken);
            if (!loadResult.Ok)
            {
                return Operation.Error(loadResult.Error);
            }

            loadResult = await LoadKpiTypesAsync(cancellationToken);
            if (!loadResult.Ok)
            {
                return Operation.Error(loadResult.Error);
            }

            loadResult =await LoadCalculationTypesAsync(cancellationToken);
            if (!loadResult.Ok)
            {
                return Operation.Error(loadResult.Error);
            }

            return true;
        }
        catch (Exception exception) 
        {
            ErrorMessage = exception.Message;
            return Operation.Error(exception.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    #region Modal
    public void OpenModal(KpiRowModel row)
    {
        SelectedKpi = row;
        IsModalOpen = true;
    }

    public void CloseModal()
    {
        IsModalOpen = false;
        SelectedKpi = null;
    }

    public async Task CloseModalAndReload()
    {
        CloseModal();
        await LoadAsync();
    }

    public void OpenCreateModal()
    {
        IsCreateModalOpen = true;
    }

    public void CloseCreateModal()
    {
        IsCreateModalOpen = false;
        SelectedKpi = null;
    }

    public async Task CloseCreateModalAndReload()
    {
        CloseCreateModal();
        await LoadAsync();
    }
    public void OpenDeleteModal(KpiRowModel row)
    {
        SelectedKpi = row;
        IsDeleteModalOpen = true;
    }
    public void CloseDeleteModal()
    {
        IsDeleteModalOpen = false;
        SelectedKpi = null;
    }
    public async Task CloseDeleteModalAndReload()
    {
        CloseDeleteModal();
        await LoadAsync();
    }

    #endregion

    #region Private Loading Helpers

    private async Task<Operation<bool, string>> LoadKpiAsync(CancellationToken cancellationToken)
    {
        Kpis.Clear();
        var kpiResult = await _mediator.Send(new GetAllKpis.Request(), cancellationToken);
        if (!kpiResult.Ok)
        {
            ErrorMessage = kpiResult.Error;
            return Operation.Error(kpiResult.Error);
        }

        var kpis = kpiResult.Result;
        Kpis = kpis.Select(x => new KpiRowModel
        {
            Title = x.Title,
            Code = x.Code,
            Abbreviation = x.Abbreviation,
            CalculationType = x.CalculationType,
            MeasurementUnit = new MeasurementUnitModel { Code = x.MeasurementUnit.Code, Title = x.MeasurementUnit.Title },
            KpiType = x.KpiType,
            CustomerFilter = x.CustomerFilter.ToList(),
            ProductFilter = x.ProductFilter.ToList(),
            CompanyFilter = x.CompanyFilter.ToList(),
        }).ToList();

        return true;
    }

    private async Task<Operation<bool, string>> LoadUnitsAsync(CancellationToken cancellationToken)
    {
        var unitsResult = await _mediator.Send(new GetAllMeasurementUnits.Request(), cancellationToken);
        if (!unitsResult.Ok)
        {
            ErrorMessage = unitsResult.Error;
            return Operation.Error(unitsResult.Error);
        }

        Units = unitsResult.Result?.Select(dto => new MeasurementUnitModel
        {
            Code = dto.Code,
            Title = dto.Title
        }).ToList() ?? [];
        return true;
    }

    private async Task<Operation<bool, string>> LoadKpiTypesAsync(CancellationToken cancellationToken)
    {
        var typesResult = await _mediator.Send(new GetAllTypes.Request(), cancellationToken);
        if (!typesResult.Ok)
        {
            ErrorMessage = typesResult.Error;
            return Operation.Error(typesResult.Error);
        }

        KpiTypes = typesResult.Result?.Select(x => new KpiTypesModel { Title = x }).ToList() ?? [];
        return true;
    }

    private async Task<Operation<bool, string>> LoadCalculationTypesAsync(CancellationToken cancellationToken)
    {
        var calculationTypeResult = await _mediator.Send(new GetAllCalculationTypes.Request(), cancellationToken);
        if (!calculationTypeResult.Ok)
        {
            ErrorMessage = calculationTypeResult.Error;
            return Operation.Error(calculationTypeResult.Error);
        }

        CalculationTypes = calculationTypeResult.Result?.Select(x => new CalculationTypeModel { Title = x }).ToList() ?? [];
        return true;
    }

    #endregion
}