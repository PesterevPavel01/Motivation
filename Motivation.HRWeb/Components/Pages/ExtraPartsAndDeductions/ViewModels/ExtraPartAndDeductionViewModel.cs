using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Messages.DeductionMessages;
using Motivation.HRWeb.Application.Messages.EmployeeMessages;
using Motivation.HRWeb.Components.Pages.ExtraPartsAndDeductions.Models;

namespace Motivation.HRWeb.Components.Pages.ExtraPartsAndDeductions.ViewModels;

public class ExtraPartAndDeductionViewModel
{
    private readonly IMediator _mediator;

    public ExtraPartAndDeductionViewModel(IMediator mediator, Action? stateChangedCallback = null) => _mediator = mediator;

    public ExtraPartModalState ModalState { get; private set; } = new();

    public ExtraPartAndDeductionFilterModel Filter { get; set; } = new();
    public List<ExtraPartAndDeductionRowModel> FilteredData { get; private set; } = [];
    public string? ErrorMessage { get; private set; }
    public bool IsLoading { get; private set; }

    public bool IsExtraPartModalOpen { get; private set; }
    public ExtraPartAndDeductionRowModel? SelectedRow { get; private set; }

    public async Task<Operation<bool, string>> LoadAsync(CancellationToken cancellationToken = default)
    {
        IsLoading = true;
        ErrorMessage = null;
        FilteredData.Clear();

        try
        {
            var result = await _mediator.Send(new GetAllEmployees.Request(), cancellationToken);
            if (!result.Ok)
            {
                ErrorMessage = result.Error;
                return Operation.Error(result.Error);
            }

            var employees = result.Result ?? Enumerable.Empty<EmployeeDto>();

            foreach (var emp in employees)
            {
                if (emp.Positions == null || !emp.Positions.Any()) continue;

                foreach (var position in emp.Positions)
                {
                    if (position.Position == null) continue;

                    var deduction = position.DeductionHistory?
                        .FirstOrDefault(h => h != null && h.Month == Filter.Month && h.Year == Filter.Year);

                    var targetDate = new DateTime(Filter.Year, Filter.Month, 1);
                    var extraPart = position.ExtraPartHistory?
                        .FirstOrDefault(x => x != null
                                             && x.ValidFrom <= targetDate
                                             && (x.ValidTo >= targetDate.AddMonths(1).AddDays(-1)));

                    FilteredData.Add(new ExtraPartAndDeductionRowModel
                    {
                        EmployeeCode = emp.Code,
                        EmployeeFullName = emp.FullName,
                        PositionCode = position.Position.Code,
                        PositionTitle = position.Position.Title,

                        DeductionCode = deduction?.Code,
                        DeductionValue = deduction?.Value,

                        CurrentExtraPart = extraPart,
                        ExtraPartValue = extraPart?.Value
                    });
                }
            }
            return true;
        }
        catch (Exception exception)
        {
            return Operation.Error(exception.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    #region Deduction
    public void StartDeductionEditing(ExtraPartAndDeductionRowModel row)
    {
        row.DeductionEditingValue = row.DeductionValue ?? 0;
        row.IsDeductionEditing = true;
    }

    public void CancelDeductionEditing(ExtraPartAndDeductionRowModel row)
    {
        row.IsDeductionEditing = false;
    }

    public async Task SaveDeductionAsync(ExtraPartAndDeductionRowModel row)
    {
        if (!row.IsDeductionEditing) return;

        try
        {
            row.IsDeductionSaving = true;

            var result = await _mediator.Send(new ManageDeduction.Request(
                DeductionCode: row.DeductionCode,
                EmployeeCode: row.EmployeeCode,
                Month: Filter.Month,
                Year: Filter.Year,
                PositionCode: row.PositionCode,
                NewValue: row.DeductionEditingValue
            ));

            if (!result.Ok)
            {
                ErrorMessage = result.Error ?? "Ошибка сохранения штрафа.";
            }
            else
            {
                row.DeductionValue = row.DeductionEditingValue;
                row.IsDeductionEditing = false;
                ErrorMessage = null;
            }
        }
        finally
        {
            row.IsDeductionSaving = false;
        }
    }
    #endregion

    #region Extra part (Modal)
    public void OpenExtraPartModal(ExtraPartAndDeductionRowModel row)
    {
        if (ModalState.IsOpen) 
            return;

        ModalState.Filter = Filter;
        ModalState.IsOpen = true;
        ModalState.EmployeeCode = row.EmployeeCode;
        ModalState.EmployeeFullName = row.EmployeeFullName;
        ModalState.PositionCode = row.PositionCode;
        ModalState.CurrentExtraPart = row.CurrentExtraPart;
    }

    public void CloseExtraPartModal()
    {
        ModalState.IsOpen = false;
    }

    public async Task CloseExtraPartModalAndReload(ExtraPartSaveResult saveResult)
    {
        CloseExtraPartModal();
        if (!string.IsNullOrWhiteSpace(saveResult.EmployeeCode) && saveResult.ExtraPart is not null)
        {
            var rowToUpdate = FilteredData.FirstOrDefault(r => r.EmployeeCode == saveResult.EmployeeCode && r.PositionCode == saveResult.ExtraPart.PositionCode);
            if (rowToUpdate is not null)
            {
                rowToUpdate.CurrentExtraPart = saveResult.ExtraPart;
                rowToUpdate.ExtraPartValue = saveResult.ExtraPart.Value;
            }
        }
        await LoadAsync();
    }
    #endregion
}