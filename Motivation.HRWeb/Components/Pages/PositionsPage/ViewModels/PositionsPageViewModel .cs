using Mediator;
using Microsoft.AspNetCore.Components;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Messages.PositionMessages;
using Motivation.HRWeb.Exceptions;

namespace Motivation.HRWeb.Components.Pages.PositionsPage.ViewModels;

public class PositionsPageViewModel : IDisposable
{
    private readonly IMediator _mediator;
    private readonly NavigationManager _navigationManager;

    public PositionsPageViewModel(IMediator mediator, NavigationManager navigationManager)
    {
        _mediator = mediator;
        _navigationManager = navigationManager;
    }

    public IEnumerable<PositionDto>? Positions { get; private set; }
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; private set; }

    public PositionDto? SelectedPosition { get; private set; }
    public bool ShowCreateModal { get; private set; }
    public bool ShowEditModal { get; private set; }
    public bool ShowDeleteModal { get; private set; }

    public event Action? OnStateChanged;
    public event Action? OnPositionsLoaded;
    public event Action<string?>? OnError;

    public async Task LoadPositionsAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        Positions = null;
        NotifyStateChanged();

        try
        {
            var queryResult = await _mediator.Send(new GetAllPositions.Request());

            if (!queryResult.Ok)
            {
                ErrorMessage = queryResult.Error ?? "Ошибка загрузки должностей.";
                OnError?.Invoke(ErrorMessage);
            }
            else
            {
                Positions = queryResult.Result;
                OnPositionsLoaded?.Invoke();
            }
        }
        catch (RedirectToLoginException)
        {
            _navigationManager.NavigateTo("/authentication/login", forceLoad: true);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            OnError?.Invoke(ErrorMessage);
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public void OpenCreateModal()
    {
        ShowCreateModal = true;
        NotifyStateChanged();
    }

    public void OpenEditModal(PositionDto position)
    {
        SelectedPosition = position;
        ShowEditModal = true;
        NotifyStateChanged();
    }

    public void OpenDeleteModal(PositionDto position)
    {
        SelectedPosition = position;
        ShowDeleteModal = true;
        NotifyStateChanged();
    }

    public void CloseModals()
    {
        ShowCreateModal = false;
        ShowEditModal = false;
        ShowDeleteModal = false;
        SelectedPosition = null;
        NotifyStateChanged();
    }

    public async Task OnPositionCreatedAsync()
    {
        CloseModals();
        await LoadPositionsAsync();
    }

    public async Task OnPositionUpdatedAsync()
    {
        CloseModals();
        await LoadPositionsAsync();
    }

    public async Task OnPositionDeletedAsync()
    {
        CloseModals();
        await LoadPositionsAsync();
    }

    public int PositionsCount => Positions?.Count() ?? 0;

    private void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }

    public void Dispose()
    {
    }
}
