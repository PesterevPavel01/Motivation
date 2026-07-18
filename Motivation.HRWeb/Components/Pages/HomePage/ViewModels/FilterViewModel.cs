using Mediator;
using Microsoft.AspNetCore.Components;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Messages.EmployeeMessages;
using Motivation.HRWeb.Exceptions;

namespace Motivation.HRWeb.Components.Pages.HomePage.ViewModels;

public class FilterViewModel : IDisposable
{
    private readonly IMediator _mediator;
    private readonly NavigationManager _navigationManager;

    public FilterViewModel(IMediator mediator, NavigationManager navigationManager)
    {
        _mediator = mediator;
        _navigationManager = navigationManager;
    }

    public class FilterModel
    {
        public int Month { get; set; } = DateTime.Today.Month;
        public int Year { get; set; } = DateTime.Today.Year;
        public string? EmployeeCode { get; set; }
        public string? PositionCode { get; set; }
        public List<EmployeeDto>? Employees { get; set; }
        public List<PositionItem>? Positions { get; set; }
    }

    public class PositionItem
    {
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }

    public FilterModel Filter { get; set; } = new();
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }

    public event Action? OnStateChanged;
    public event Action? OnPositionsUpdated;
    public event Action? OnFilterApplied;

    public async Task LoadEmployeesAsync()
    {
        IsLoading = true;
        NotifyStateChanged();

        try
        {
            var result = await _mediator.Send(new GetAllEmployees.Request());
            if (result.Ok && result.Result != null)
            {
                Filter.Employees = result.Result.ToList();
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

    public void OnEmployeeChanged()
    {
        Filter.Positions = new List<PositionItem>();
        Filter.PositionCode = null;

        if (string.IsNullOrEmpty(Filter.EmployeeCode))
        {
            OnPositionsUpdated?.Invoke();
            NotifyStateChanged();
            return;
        }

        var employee = Filter.Employees?.FirstOrDefault(e => e.Code == Filter.EmployeeCode);
        if (employee == null)
        {
            OnPositionsUpdated?.Invoke();
            NotifyStateChanged();
            return;
        }

        Filter.Positions = employee.Positions?
            .Where(p => p.Position != null)
            .Select(p => new PositionItem
            {
                Code = p.Position!.Code ?? string.Empty,
                Title = p.Position!.Title ?? "Неизвестно"
            })
            .DistinctBy(p => p.Code)
            .ToList() ?? [];

        OnPositionsUpdated?.Invoke();
        NotifyStateChanged();
    }

    public void ApplyFilter()
    {
        OnFilterApplied?.Invoke();
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();

    public void Dispose() { }
}