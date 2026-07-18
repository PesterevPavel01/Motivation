using Mediator;
using Motivation.HRWeb.Application.Mappings;
using Motivation.HRWeb.Application.Messages.EmployeeMessages;
using Motivation.HRWeb.Components.Pages.Employees.Models;
using Motivation.HRWeb.Components.Pages.EmployeesPage.Models;

namespace Motivation.HRWeb.Components.Pages.EmployeesPage.ViewModels;

public class EditEmployeeModalViewModel
{
    private readonly IMediator _mediator;

    public EditEmployeeModalViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public EditEmployeeModel Model { get; } = new();
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; set; }
    public event Action? OnStateChanged;

    public void LoadEmployee(EmployeeItemModel employee)
    {
        Model.LoadFrom(employee);
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public async Task<bool> UpdateAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;
            NotifyStateChanged();

            var dto = Model.ToUpdateDto();
            var result = await _mediator.Send(new UpdateEmployee.Request(dto));

            if (!result.Ok)
            {
                ErrorMessage = result.Error ?? "Ошибка обновления сотрудника";
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