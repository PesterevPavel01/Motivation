using Mediator;
using Motivation.HRWeb.Application.Messages.EmployeeMessages;
using Motivation.HRWeb.Components.Pages.EmployeesPage.Models;

namespace Motivation.HRWeb.Components.Pages.EmployeesPage.ViewModels;

public class DismissEmployeeModalViewModel
{
    private readonly IMediator _mediator;

    public DismissEmployeeModalViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public DismissEmployeeModel Model { get; } = new();
    public EmployeeItemModel? Employee { get; private set; }
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; set; }
    public event Action? OnStateChanged;

    public void LoadEmployee(EmployeeItemModel employee)
    {
        Employee = employee;
        Model.Reset();
        Model.SetAvailablePositions(employee.Positions);
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public async Task<bool> DismissAsync()
    {
        if (Employee == null || string.IsNullOrEmpty(Model.SelectedPositionCode))
            return false;

        try
        {
            IsLoading = true;
            ErrorMessage = null;
            NotifyStateChanged();

            var result = await _mediator.Send(new DismissEmployeeFromPosition.Request(
                Employee.Code,
                Model.SelectedPositionCode));

            if (!result.Ok)
            {
                ErrorMessage = result.Error ?? "Ошибка увольнения с должности";
                NotifyStateChanged();
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotifyStateChanged();
            return false;
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}