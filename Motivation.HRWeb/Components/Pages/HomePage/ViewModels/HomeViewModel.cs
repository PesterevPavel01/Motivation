using Mediator;
using Microsoft.AspNetCore.Components;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application;
using Motivation.HRWeb.Application.Mappings;
using Motivation.HRWeb.Application.Messages.EmployeeMessages;
using Motivation.HRWeb.Application.Messages.TimeStandardHours;
using Motivation.HRWeb.Components.Pages.HomePage.Helpers;
using Motivation.HRWeb.Components.Pages.HomePage.Models;
using Motivation.HRWeb.Exceptions;

namespace Motivation.HRWeb.Components.Pages.HomePage.ViewModels;

public class HomeViewModel : IDisposable
{
    private readonly IMediator _mediator;
    private readonly NavigationManager _navigationManager;

    public HomeViewModel(IMediator mediator, NavigationManager navigationManager)
    {
        _mediator = mediator;
        _navigationManager = navigationManager;
    }

    public List<HomeModel> LeftTableData { get; set; } = [];
    public List<KpiRowModel> KpiTableData { get; set; } = [];
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public event Action? OnStateChanged;

    public async Task LoadDataAsync(
        string employeeCode,
        string positionCode,
        int month,
        int year)
    {
        if (string.IsNullOrEmpty(employeeCode))
        {
            ErrorMessage = "Выберите сотрудника";
            NotifyStateChanged();
            return;
        }

        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            var employeeResult = await _mediator.Send(new GetAllEmployees.Request());
            if (!employeeResult.Ok || employeeResult.Result == null)
            {
                ErrorMessage = "Не удалось загрузить данные сотрудника";
                return;
            }

            var employee = employeeResult.Result.FirstOrDefault(e => e.Code == employeeCode);
            if (employee == null)
            {
                ErrorMessage = "Сотрудник не найден";
                return;
            }

            var position = employee.Positions?.FirstOrDefault(p => p.Position?.Code == positionCode);
            if (position == null)
            {
                ErrorMessage = "Должность не найдена";
                return;
            }

            await BuildLeftTableAsync(employee, position, month, year);
            await BuildKpiTableAsync(employee, position, month, year);

            NotifyStateChanged();
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

    public async Task EditFactCellCompleted(EditableFactCellSaveResult result)
    {
        if (result.Success)
        {
            result.Row.Fact = result.NewValue;
        }
        else
        {
            ErrorMessage = result.ErrorMessage;
        }
        NotifyStateChanged();
        await Task.CompletedTask;
    }

    public void ClearMessages()
    {
        ErrorMessage = null;
        SuccessMessage = null;
        NotifyStateChanged();
    }

    private async Task BuildLeftTableAsync(EmployeeDto employee, EmployeePositionDto position, int month, int year)
    {
        var standardHours = await GetStandardHoursAsync(position, month, year);
        var actualHours = HomeHelper.GetActualHours(position?.ActualHoursHistory!, month, year);
        var extraPart = HomeHelper.GetExtraPart(position?.ExtraPartHistory!, month, year);
        var baseSalary = position?.Position?.BaseSalary ?? 0;
        var motivationPart = position?.Position?.MotivationPart ?? 0;
        var salary = HomeHelper.CalculateSalary(baseSalary, actualHours, standardHours);

        LeftTableData = new List<HomeModel>
        {
            new() { Parameter = "Норма часов", Value = ValueFormatter.FormatValue(standardHours) },
            new() { Parameter = "Отработано часов", Value = ValueFormatter.FormatValue(actualHours) },
            new() { Parameter = "Оклад", Value = ValueFormatter.FormatValue(baseSalary) },
            new() { Parameter = "Надбавка", Value = ValueFormatter.FormatValue(extraPart) },
            new() { Parameter = "Итого по окладу", Value = ValueFormatter.FormatValue(salary) },
            new() { Parameter = "Премия", Value = ValueFormatter.FormatValue(motivationPart) }
        };
    }

    private async Task BuildKpiTableAsync(EmployeeDto employee, EmployeePositionDto position, int month, int year)
    {
        var motivationPart = position.Position?.MotivationPart ?? 0;
        var kpis = position.Position?.Kpis ?? [];

        KpiTableData = kpis
            .ToKpiRowModels(position, month, year, motivationPart)
            .ToList();

        await Task.CompletedTask;
    }

    private async Task<decimal?> GetStandardHoursAsync(EmployeePositionDto position, int month, int year)
    {
        var workWeekType = position.Position?.WorkWeekType;
        if (string.IsNullOrEmpty(workWeekType)) return null;

        var result = await _mediator.Send(new GetStandardHours.Request(month, year, workWeekType));
        return result.Ok ? result.Result?.Value : null;
    }

    public void NotifyStateChanged() => OnStateChanged?.Invoke();

    public void Dispose() { }
}