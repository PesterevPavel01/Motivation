using Mediator;
using Microsoft.AspNetCore.Components;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Components.Pages.PositionsPage.ViewModels;

namespace Motivation.HRWeb.Components.Pages.PositionsPage.Modals;

public partial class DeletePositionModal : ComponentBase
{
    [Inject] private IMediator Mediator { get; set; } = null!;

    [Parameter] public PositionDto? Position { get; set; }
    [Parameter] public EventCallback OnDeleted { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private DeletePositionModalViewModel ViewModel { get; set; } = null!;

    protected override void OnInitialized()
    {
        ViewModel = new DeletePositionModalViewModel(Mediator);
        ViewModel.OnStateChanged += StateHasChanged;
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        if (Position != null)
        {
            ViewModel.InitializeFromPosition(Position);
        }
    }

    private async Task HandleDelete()
    {
        var result = await ViewModel.DeletePositionAsync();
        if (result)
        {
            await OnDeleted.InvokeAsync();
        }
    }
}