using Mediator;
using Microsoft.AspNetCore.Components;
using Motivation.HRWeb.Components.Pages.PositionsPage.ViewModels;

namespace Motivation.HRWeb.Components.Pages.PositionsPage;

public partial class Positions : ComponentBase, IDisposable
{
    [Inject] private IMediator Mediator { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    private PositionsPageViewModel ViewModel { get; set; } = null!;

    protected override void OnInitialized()
    {
        InitializeViewModel();
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ViewModel.LoadPositionsAsync();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void InitializeViewModel()
    {
        ViewModel = new PositionsPageViewModel(Mediator, NavigationManager);
        ViewModel.OnStateChanged += StateHasChanged;
        ViewModel.OnError += (error) => StateHasChanged();
        ViewModel.OnPositionsLoaded += () => StateHasChanged();
    }

    public void Dispose()
    {
        ViewModel?.Dispose();
        GC.SuppressFinalize(this);
    }
}