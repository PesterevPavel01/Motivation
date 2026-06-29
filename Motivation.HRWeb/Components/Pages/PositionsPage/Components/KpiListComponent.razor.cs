using Microsoft.AspNetCore.Components;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Validators;
using Motivation.HRWeb.Components.Pages.PositionsPage.ViewModels;

namespace Motivation.HRWeb.Components.Pages.PositionsPage.Components;

public partial class KpiListComponent : ComponentBase
{
    [Parameter] public List<PositionKpiItemModel> Items { get; set; } = [];
    [Parameter] public List<KpiDto> AvailableKpis { get; set; } = [];
    [Parameter] public EventCallback OnKpisChanged { get; set; }

    private readonly IKpiValidator _validator;

    public KpiListComponent(IKpiValidator validator)
    {
        _validator = validator;
    }

    private KpiListComponentViewModel ViewModel { get; set; } = null!;

    protected override void OnInitialized()
    {
        ViewModel = new KpiListComponentViewModel(_validator);
        ViewModel.OnStateChanged += StateHasChanged;

        ViewModel.Items = Items;
        ViewModel.AvailableKpis = AvailableKpis;

        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        ViewModel.Items = Items;
        ViewModel.AvailableKpis = AvailableKpis;
        ViewModel.NotifyStateChanged();
    }
}