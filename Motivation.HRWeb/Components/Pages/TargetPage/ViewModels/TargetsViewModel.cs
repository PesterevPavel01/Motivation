using Mediator;
using Microsoft.AspNetCore.Components;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Messages.EmployeeMessages;
using Motivation.HRWeb.Application.Messages.KpiTarget;
using Motivation.HRWeb.Components.Pages.TargetPage.Helpers;
using Motivation.HRWeb.Components.Pages.TargetPage.Models;
using Motivation.HRWeb.Exceptions;
using NetTopologySuite.Index.HPRtree;

namespace Motivation.HRWeb.Components.Pages.TargetPage.ViewModels;

public class TargetsViewModel : IDisposable
{
    private readonly IMediator _mediator;
    private readonly NavigationManager _navigationManager;

    public TargetsViewModel(IMediator mediator, NavigationManager navigationManager)
    {
        _mediator = mediator;
        _navigationManager = navigationManager;
    }

    public TargetsFilterModel Filter { get; set; } = new();
    public List<KpiTargetItemModel> Items { get; set; } = new();
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }

    public List<int> Months => Enumerable.Range(1, 12).ToList();
    public List<int> Years => Enumerable.Range(2020, DateTime.Now.Year - 2020 + 1).ToList();

    public event Action? OnStateChanged;
    public event Action? OnEmployeesLoaded;
    public event Action<string?>? OnError;

    public async Task LoadEmployeesAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            var result = await _mediator.Send(new GetAllEmployees.Request());
            if (result.Ok && result.Result is not null)
            {
                Filter.Employees = result.Result.ToList();
                OnEmployeesLoaded?.Invoke();
            }
            else
            {
                ErrorMessage = result.Error ?? "Ошибка загрузки сотрудников";
                OnError?.Invoke(ErrorMessage);
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

    public async Task ApplyFilterAsync()
    {
        if (string.IsNullOrEmpty(Filter.EmployeeCode))
        {
            Items.Clear();
            ErrorMessage = "Пожалуйста, выберите сотрудника";
            NotifyStateChanged();
            return;
        }

        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            var result = await _mediator.Send(new GetEmployeeKpiTergets.Request(Filter.EmployeeCode));
            if (!result.Ok)
            {
                ErrorMessage = result.Error;
                Items.Clear();
                NotifyStateChanged();
                return;
            }

            var employee = result.Result;
            Items.Clear();

            Items = (employee.Positions ?? Enumerable.Empty<EmployeePositionDto>())
                .SelectMany(position => (position.Position?.Kpis ?? Enumerable.Empty<PositionKpiDto>())
                    .Select(kpi => new KpiTargetItemModel
                    {
                        EmployeeCode = employee.Code ?? string.Empty,
                        EmployeeTitle = employee.FullName ?? employee.LastName ?? "Сотрудник",
                        PositionCode = position.Position?.Code ?? string.Empty,
                        PositionTitle = position.Position?.Title ?? "Должность",
                        KpiCode = kpi.KpiCode ?? string.Empty,
                        KpiTitle = kpi.KpiTitle ?? "KPI",
                        OrderNumber = kpi.OrderNumber,
                        Target = KpiTargetHelper.GetTargetForPeriod(kpi.Targets, Filter.Year, Filter.Month),
                        IsEditing = false,
                        EditValue = null,
                        IsSaving = false
                    }))
                .OrderBy(x => x.PositionCode)
                .ThenBy(x => x.OrderNumber)
                .ToList();

            if (!Items.Any())
                ErrorMessage = "У выбранного сотрудника нет KPI";

            NotifyStateChanged();
        }
        catch (RedirectToLoginException)
        {
            _navigationManager.NavigateTo("/authentication/login", forceLoad: true);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки: {ex.Message}";
            OnError?.Invoke(ErrorMessage);
            NotifyStateChanged();
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public void StartEditing(KpiTargetItemModel item)
    {
        item.IsEditing = true;
        item.EditValue = item.Target;
        NotifyStateChanged();
    }

    public void CancelEditing(KpiTargetItemModel item)
    {
        item.IsEditing = false;
        item.EditValue = item.Target;
        NotifyStateChanged();
    }

    public async Task SaveTargetAsync(KpiTargetItemModel item)
    {
        if (item.IsSaving) return;

        try
        {
            item.IsSaving = true;
            NotifyStateChanged();

            if (item.EditValue == item.Target)
            {
                item.IsEditing = false;
                NotifyStateChanged();
                return;
            }

            var model = new SetEmployeeKpiTarget
            {
                EmployeeCode = item.EmployeeCode,
                KpiCode = item.KpiCode,
                TargetValue = item.EditValue ?? 0,
                ValidFrom = new DateTime(Filter.Year, Filter.Month, 1),
                ValidTo = new DateTime(Filter.Year, Filter.Month, DateTime.DaysInMonth(Filter.Year, Filter.Month), 23, 59, 59)
            };

            var result = await _mediator.Send(new SetKpiTarget.Request(model));
            if (!result.Ok)
            {
                ErrorMessage = result.Error ?? "Ошибка сохранения цели";
                OnError?.Invoke(ErrorMessage);
                NotifyStateChanged();
                return;
            }

            item.Target = item.EditValue;
            item.IsEditing = false;
            await ApplyFilterAsync();
        }
        catch (RedirectToLoginException)
        {
            _navigationManager.NavigateTo("/authentication/login", forceLoad: true);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            OnError?.Invoke(ErrorMessage);
            NotifyStateChanged();
        }
        finally
        {
            item.IsSaving = false;
            NotifyStateChanged();
        }
    }

    public int ItemsCount => Items?.Count ?? 0;

    private void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }

    public void Dispose()
    {
    }
}