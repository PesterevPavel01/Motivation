using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Validators;
using Motivation.HRWeb.Components.Pages.PositionsPage.Models;

namespace Motivation.HRWeb.Components.Pages.PositionsPage.ViewModels;

public class KpiListComponentViewModel
{
    private readonly IKpiValidator _validator;
    public KpiListComponentViewModel(IKpiValidator validator)
    {
        _validator = validator;
        AvailableKpis = new List<KpiDto>();
        Items = new List<PositionKpiItemModel>();
        NewKpi = new PositionKpiItemModel();
    }

    public List<PositionKpiItemModel> Items { get; set; }
    public List<KpiDto> AvailableKpis { get; set; }
    public PositionKpiItemModel NewKpi { get; set; }
    public bool ShowNewRow { get; set; }
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; private set; }

    public event Action? OnStateChanged;

    public void ShowAddRow()
    {
        NewKpi = new PositionKpiItemModel
        {
            IsNew = true
        };
        ShowNewRow = true;
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public void CancelNewRow()
    {
        NewKpi = new PositionKpiItemModel();
        ShowNewRow = false;
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public void AddNewKpi()
    {
        var validationResult = _validator.ValidateNewKpi(NewKpi, Items);
        if (!validationResult.Ok)
        {
            ErrorMessage = validationResult.Error.FirstOrDefault();
            NotifyStateChanged();
            return;
        }

        var selectedKpi = AvailableKpis.FirstOrDefault(x => x.Code == NewKpi.KpiCode);
        if (selectedKpi != null)
        {
            NewKpi.KpiTitle = selectedKpi.Title;
        }

        NewKpi.IsNew = false;
        Items.Add(NewKpi);
        NewKpi = new PositionKpiItemModel();
        ShowNewRow = false;
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public void StartEditItem(PositionKpiItemModel item)
    {
        item.IsEditing = true;
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public void SaveEditItem(PositionKpiItemModel item)
    {
        var validationResult = _validator.ValidateEditItem(item, Items);
        if (!validationResult.Ok)
        {
            ErrorMessage = validationResult.Error.FirstOrDefault();
            NotifyStateChanged();
            return;
        }

        var selectedKpi = AvailableKpis.FirstOrDefault(x => x.Code == item.KpiCode);
        if (selectedKpi != null)
        {
            item.KpiTitle = selectedKpi.Title;
        }

        item.IsEditing = false;
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public void CancelEditItem(PositionKpiItemModel item)
    {
        item.IsEditing = false;
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public void RemoveItem(PositionKpiItemModel item)
    {
        Items.Remove(item);
        NotifyStateChanged();
    }

    public void SaveNewItem(PositionKpiItemModel item)
    {
        var validationResult = _validator.ValidateEditItem(item, Items);
        var selectedKpi = AvailableKpis.FirstOrDefault(x => x.Code == item.KpiCode);
        if (selectedKpi != null)
        {
            item.KpiTitle = selectedKpi.Title;
        }

        item.IsNew = false;
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public void CancelNewItem(PositionKpiItemModel item)
    {
        Items.Remove(item);
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }
}