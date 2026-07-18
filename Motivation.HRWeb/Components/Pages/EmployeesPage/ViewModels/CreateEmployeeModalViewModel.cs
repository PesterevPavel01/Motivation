using Mediator;
using Motivation.HRWeb.Application.Mappings;
using Motivation.HRWeb.Application.Messages.EmployeeMessages;
using Motivation.HRWeb.Components.Pages.Employees.Models;

namespace Motivation.HRWeb.Components.Pages.EmployeesPage.ViewModels;

public class CreateEmployeeModalViewModel
{
    private readonly IMediator _mediator;

    public CreateEmployeeModalViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public CreateEmployeeModel Model { get; set; } = new();
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; set; }
    public event Action? OnStateChanged;

    public async Task<bool> CreateAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;
            NotifyStateChanged();

            var dto = Model.ToCreateDto();
            var result = await _mediator.Send(new CreateEmployee.Request(dto));

            if (!result.Ok)
            {
                ErrorMessage = result.Error ?? "Ошибка создания сотрудника";
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

    public void Reset()
    {
        Model.Reset();
        ErrorMessage = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}