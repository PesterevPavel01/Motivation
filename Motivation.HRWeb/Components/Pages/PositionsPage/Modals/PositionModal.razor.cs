using Mediator;
using Microsoft.AspNetCore.Components;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Components.Pages.PositionsPage.ViewModels;

namespace Motivation.HRWeb.Components.Pages.PositionsPage.Modals;

public partial class PositionModal : ComponentBase
{
    [Inject] private IMediator Mediator { get; set; } = null!;

    [Parameter] public PositionDto? Position { get; set; }
    [Parameter] public bool IsEditMode { get; set; }
    [Parameter] public EventCallback OnSaved { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private PositionModalViewModel ViewModel { get; set; } = null!;

    protected override void OnInitialized()
    {
        ViewModel = new PositionModalViewModel(Mediator);
        ViewModel.OnStateChanged += StateHasChanged;
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        ViewModel.SetMode(IsEditMode);

        if (IsEditMode && Position != null)
        {
            ViewModel.InitializeFromPosition(Position);
        }
        else
        {
            ViewModel.Reset();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && ViewModel.AvailableKpis.Count == 0)
        {
            await ViewModel.LoadAvailableKpisAsync();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleSave()
    {
        var result = await ViewModel.SavePositionAsync();
        if (result)
        {
            await OnSaved.InvokeAsync();
        }
    }
}