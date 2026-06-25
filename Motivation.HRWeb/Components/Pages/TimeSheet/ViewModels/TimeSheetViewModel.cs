using Mediator;
using Motivation.Contracts.Dto;
using Motivation.Domain.Enum;
using Motivation.HRWeb.Application.Messages.EmployeeMessages;
using Motivation.HRWeb.Application.Messages.TimeSheetMessages;
using Motivation.HRWeb.Application.Messages.TimeStandardHours;
using Motivation.Application.Mappings;
using Motivation.HRWeb.Components.Pages.TimeSheet.Modes;
using Calabonga.OperationResults;
using Motivation.Contracts;
using System.Net.NetworkInformation;

namespace Motivation.HRWeb.Components.Pages.TimeSheet.ViewModels;

public class TimeSheetViewModel
{
    private readonly IMediator _mediator;

    public TimeSheetViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public TimeSheetFilterModel Filter { get; set; } = new();

    public List<StandardHoursModel> StandardHoursList { get; private set; } = [];
    public List<TimeSheetRowModel> FilteredData { get; private set; } = [];

    public string? ErrorMessage { get; private set; }
    public bool IsLoading { get; private set; }

    public string? EditingRowKey { get; private set; }

    public IEnumerable<WorkWeekType> WeekTypes => Enum.GetValues<WorkWeekType>();


    public async Task<Operation<bool, string>> LoadAsync(CancellationToken cancellationToken = default)
    {
        IsLoading = true;
        ErrorMessage = null;
        FilteredData.Clear();
        StandardHoursList.Clear();
        EditingRowKey = null;

        try
        {
            var loadStandardHours = await LoadStandardHours(cancellationToken);
            if (!loadStandardHours.Ok)
            {
                ErrorMessage = loadStandardHours.Error;
                return Operation.Error(loadStandardHours.Error);
            }

            var loadEmployees = await LoadEmployees(cancellationToken);
            if (!loadEmployees.Ok)
            {
                ErrorMessage = loadEmployees.Error;
                return Operation.Error(loadEmployees.Error);
            }

        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
            return Operation.Error(exception.Message);
        }
        finally
        {
            IsLoading = false;
        }
        return true;
    }

    #region Standard hours

    private async Task<Operation<bool, string>> LoadStandardHours(CancellationToken cancellationToken)
    {
        foreach (var weekType in Enum.GetValues<WorkWeekType>())
        {
            var weekTypeRussian = weekType.ToRussianString();
            var standardResult = await _mediator.Send(
                new GetStandardHours.Request(Filter.Month, Filter.Year, weekTypeRussian),
                cancellationToken);

            if (!standardResult.Ok && standardResult.Error.Replace("\"", "").Trim() != ErrorMessages.NotFound)
            {
                return Operation.Error(standardResult.Error);
            }

            var newItem = new StandardHoursModel
            {
                WorkWeekType = weekTypeRussian,
                IsEditing = false,
                IsSaving = false
            };

            if (standardResult.Result is not null)
            {
                newItem.Id = standardResult.Result.Id;
                newItem.Value = standardResult.Result.Value;
            }

            StandardHoursList.Add(newItem);
        }

        return true;
    }

    public void StartEditingStandardHours(string weekTypeRussian)
    {
        var item = StandardHoursList.FirstOrDefault(x => x.WorkWeekType == weekTypeRussian);
        if (item != null) item.IsEditing = true;
    }

    public void CancelStandardHours(string weekTypeRussian)
    {
        var item = StandardHoursList.FirstOrDefault(x => x.WorkWeekType == weekTypeRussian);
        if (item != null)
        {
            item.IsEditing = false;
            item.Value = item.Id.HasValue ? item.Value : 0;
        }
    }

    public async Task<bool> SaveStandardHoursAsync(string weekTypeRussian)
    {
        var item = StandardHoursList.FirstOrDefault(x => x.WorkWeekType == weekTypeRussian);
        if (item == null) return false;

        try
        {
            item.IsSaving = true;

            if (item.Id.HasValue)
            {
                var dto = new StandardHoursUpdateDto(item.Value);
                var updateResult = await _mediator.Send(
                    new UpdateStandardHours.Request(item.Id.Value, dto));

                if (!updateResult.Ok)
                {
                    ErrorMessage = updateResult.Error;
                    return false;
                }
            }
            else
            {
                var dto = new StandardHoursCreateDto(
                    Value: item.Value,
                    Month: Filter.Month,
                    Year: Filter.Year,
                    WorkWeekType: item.WorkWeekType
                );

                var createResult = await _mediator.Send(new CreateStandardHours.Request(dto));
                if (createResult.Ok)
                {
                    item.Id = createResult.Result!.Id;
                }
                else
                {
                    ErrorMessage = createResult.Error;
                    return false;
                }
            }

            item.IsEditing = false;
            ErrorMessage = null;
            return true;
        }
        finally
        {
            item.IsSaving = false;
        }
    }
    #endregion

    #region Table Employees

    private async Task<Operation<bool, string>> LoadEmployees(CancellationToken cancellationToken) 
    {
        var result = await _mediator.Send(new GetAllEmployees.Request(), cancellationToken);
        if (!result.Ok)
        {
            return Operation.Error(result.Error);
        }

        var employees = result.Result ?? [];

        foreach (var emp in employees)
        {
            if (emp.Positions == null || !emp.Positions.Any()) continue;

            foreach (var pos in emp.Positions)
            {
                if (pos.Position == null) continue;

                var actualHours = pos.ActualHoursHistory?
                    .FirstOrDefault(h => h != null && h.Month == Filter.Month && h.Year == Filter.Year);

                FilteredData.Add(new TimeSheetRowModel
                {
                    EmployeeCode = emp.Code,
                    EmployeeFullName = emp.FullName,
                    PositionCode = pos.Position.Code,
                    PositionTitle = pos.Position.Title,
                    Value = actualHours?.Value
                });
            }
        }

        return true;
    }

    public void StartEditing(TimeSheetRowModel row)
    {
        var key = GetRowKey(row);
        if (EditingRowKey == key) return;

        EditingRowKey = null;
        row.EditingValue = row.Value ?? 0;
        EditingRowKey = key;
    }

    public void CancelEditing(TimeSheetRowModel row)
    {
        if (GetRowKey(row) == EditingRowKey)
        {
            EditingRowKey = null;
        }
    }

    public async Task<bool> SaveActualHoursAsync(TimeSheetRowModel row)
    {
        var key = GetRowKey(row);
        if (key != EditingRowKey) return false;

        try
        {
            row.IsSaving = true;

            var dto = new SetActualHoursDto(
                PositionCode: row.PositionCode,
                Value: row.EditingValue,
                Month: Filter.Month,
                Year: Filter.Year
            );

            var result = await _mediator.Send(new SetActualHours.Request(row.EmployeeCode, dto));

            if (!result.Ok)
            {
                ErrorMessage = result.Error ?? "Ошибка сохранения часов.";
                return false;
            }

            row.Value = row.EditingValue;
            EditingRowKey = null;
            ErrorMessage = null;
            return true;
        }
        finally
        {
            row.IsSaving = false;
        }
    }

    private string GetRowKey(TimeSheetRowModel row) => $"{row.EmployeeCode}_{row.PositionCode}";
#endregion
}