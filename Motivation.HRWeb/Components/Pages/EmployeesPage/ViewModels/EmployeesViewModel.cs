using Mediator;
using Microsoft.AspNetCore.Components;
using Motivation.HRWeb.Application.Messages.EmployeeMessages;
using Motivation.HRWeb.Components.Pages.EmployeesPage.Models;
using Motivation.HRWeb.Exceptions;

namespace Motivation.HRWeb.Components.Pages.EmployeesPage.ViewModels;

public class EmployeesViewModel : IDisposable
{
    private readonly IMediator _mediator;
    private readonly NavigationManager _navigationManager;

    // Модальные ViewModel
    public CreateEmployeeModalViewModel CreateModalViewModel { get; }
    public EditEmployeeModalViewModel EditModalViewModel { get; }
    public DeleteEmployeeModalViewModel DeleteModalViewModel { get; }
    public AssignEmployeeModalViewModel AssignModalViewModel { get; }
    public DismissEmployeeModalViewModel DismissModalViewModel { get; }

    public EmployeesViewModel(
        IMediator mediator,
        NavigationManager navigationManager,
        CreateEmployeeModalViewModel createModalViewModel,
        EditEmployeeModalViewModel editModalViewModel,
        DeleteEmployeeModalViewModel deleteModalViewModel,
        AssignEmployeeModalViewModel assignModalViewModel,
        DismissEmployeeModalViewModel dismissModalViewModel)
    {
        _mediator = mediator;
        _navigationManager = navigationManager;

        CreateModalViewModel = createModalViewModel;
        EditModalViewModel = editModalViewModel;
        DeleteModalViewModel = deleteModalViewModel;
        AssignModalViewModel = assignModalViewModel;
        DismissModalViewModel = dismissModalViewModel;
    }

    // Основные данные
    public List<EmployeeItemModel> Employees { get; private set; } = new();
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; set; }

    // Состояние модальных окон
    public bool ShowCreateModal { get; private set; }
    public bool ShowEditModal { get; private set; }
    public bool ShowDeleteModal { get; private set; }
    public bool ShowAssignModal { get; private set; }
    public bool ShowDismissModal { get; private set; }

    public EmployeeItemModel? SelectedEmployee { get; private set; }

    public event Action? OnStateChanged;

    #region Загрузка данных

    public async Task LoadEmployeesAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            var result = await _mediator.Send(new GetAllEmployees.Request());
            if (!result.Ok)
            {
                ErrorMessage = result.Error ?? "Ошибка загрузки сотрудников";
                Employees = new List<EmployeeItemModel>();
            }
            else
            {
                Employees = result.Result?
                    .Select(e => new EmployeeItemModel
                    {
                        Code = e.Code,
                        FirstName = e.FirstName,
                        SecondName = e.SecondName,
                        LastName = e.LastName,
                        FullName = e.FullName,
                        Positions = e.Positions?
                            .Select(p => new EmployeePositionItem
                            {
                                PositionCode = p.Position?.Code ?? string.Empty,
                                PositionTitle = p.Position?.Title ?? "без должности",
                                AssignmentDate = p.AssignmentDate
                            })
                            .ToList() ?? new List<EmployeePositionItem>()
                    })
                    .ToList() ?? new List<EmployeeItemModel>();
            }
        }
        catch (RedirectToLoginException)
        {
            _navigationManager.NavigateTo("/authentication/login", forceLoad: true);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    #endregion

    #region Открытие модальных окон

    public void OpenCreateModal()
    {
        CreateModalViewModel.Reset();
        ShowCreateModal = true;
        NotifyStateChanged();
    }

    public void OpenEditModal(EmployeeItemModel employee)
    {
        SelectedEmployee = employee;
        EditModalViewModel.LoadEmployee(employee);
        ShowEditModal = true;
        NotifyStateChanged();
    }

    public void OpenDeleteModal(EmployeeItemModel employee)
    {
        SelectedEmployee = employee;
        DeleteModalViewModel.LoadEmployee(employee);
        ShowDeleteModal = true;
        NotifyStateChanged();
    }

    public async Task OpenAssignModalAsync(EmployeeItemModel employee)
    {
        SelectedEmployee = employee;
        AssignModalViewModel.LoadEmployee(employee);
        await AssignModalViewModel.LoadPositionsAsync();
        ShowAssignModal = true;
        NotifyStateChanged();
    }

    public async Task OpenDismissModalAsync(EmployeeItemModel employee)
    {
        SelectedEmployee = employee;
        DismissModalViewModel.LoadEmployee(employee);
        ShowDismissModal = true;
        NotifyStateChanged();
    }

    #endregion

    #region Закрытие модальных окон

    public void CloseModals()
    {
        ShowCreateModal = false;
        ShowEditModal = false;
        ShowDeleteModal = false;
        ShowAssignModal = false;
        ShowDismissModal = false;
        SelectedEmployee = null;
        NotifyStateChanged();
    }

    #endregion

    #region Обработчики сохранения

    public async Task OnEmployeeCreatedAsync()
    {
        CloseModals();
        await LoadEmployeesAsync();
    }

    public async Task OnEmployeeUpdatedAsync()
    {
        CloseModals();
        await LoadEmployeesAsync();
    }

    public async Task OnEmployeeDeletedAsync()
    {
        CloseModals();
        await LoadEmployeesAsync();
    }

    public async Task OnEmployeeAssignedAsync()
    {
        CloseModals();
        await LoadEmployeesAsync();
    }

    public async Task OnEmployeeDismissedAsync()
    {
        CloseModals();
        await LoadEmployeesAsync();
    }

    #endregion

    #region Вспомогательные методы

    public bool HasPositions(EmployeeItemModel employee)
        => employee?.Positions?.Any() == true;

    public int EmployeesCount => Employees?.Count ?? 0;

    #endregion

    private void NotifyStateChanged() => OnStateChanged?.Invoke();

    public void Dispose() { }
}