using Mediator;
using Motivation.HRWeb.Application.Messages.EmployeeMessages;
using Motivation.HRWeb.Components.Pages.EmployeesPage.Models;

namespace Motivation.HRWeb.Components.Pages.EmployeesPage.ViewModels;

public class DeleteEmployeeModalViewModel
{
    private readonly IMediator _mediator;

    public DeleteEmployeeModalViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public EmployeeItemModel? Employee { get; private set; }
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; set; }
    public event Action? OnStateChanged;

    public void LoadEmployee(EmployeeItemModel employee)
    {
        Employee = employee;
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public async Task<bool> DeleteAsync()
    {
        if (Employee == null) return false;

        try
        {
            IsLoading = true;
            ErrorMessage = null;
            NotifyStateChanged();

            var result = await _mediator.Send(new DeleteEmployee.Request(Employee.Code));

            if (!result.Ok)
            {
                ErrorMessage = result.Error ?? "Ошибка удаления сотрудника";
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