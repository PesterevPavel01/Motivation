using Mediator;
using Microsoft.AspNetCore.Components;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Messages.EmployeeMessages;
using Motivation.HRWeb.Application.Messages.PositionMessages;
using Motivation.HRWeb.Application.Mappings;
using Motivation.HRWeb.Components.Pages.EmployeesPage.Models;

namespace Motivation.HRWeb.Components.Pages.EmployeesPage.ViewModels;

public class AssignEmployeeModalViewModel
{
    private readonly IMediator _mediator;

    public AssignEmployeeModalViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public AssignEmployeeModel Model { get; } = new();
    public EmployeeItemModel? Employee { get; private set; }
    public List<PositionDto> Positions { get; private set; } = [];
    public bool IsLoading { get; private set; }
    public bool IsLoadingPositions { get; private set; }
    public string? ErrorMessage { get; set; }
    public event Action? OnStateChanged;

    public async Task LoadPositionsAsync()
    {
        IsLoadingPositions = true;
        NotifyStateChanged();

        try
        {
            var result = await _mediator.Send(new GetAllPositions.Request());
            if (result.Ok)
                Positions = result.Result?.ToList() ?? new List<PositionDto>();
        }
        finally
        {
            IsLoadingPositions = false;
            NotifyStateChanged();
        }
    }

    public void LoadEmployee(EmployeeItemModel employee)
    {
        Employee = employee;
        Model.Reset();
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public async Task<bool> AssignAsync()
    {
        if (Employee == null || string.IsNullOrEmpty(Model.SelectedPositionCode))
            return false;

        try
        {
            IsLoading = true;
            ErrorMessage = null;
            NotifyStateChanged();

            var dto = Model.ToAssignDto();
            var result = await _mediator.Send(new AssignEmployeeToPosition.Request(Employee.Code, dto));

            if (!result.Ok)
            {
                ErrorMessage = result.Error ?? "Ошибка назначения на должность";
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