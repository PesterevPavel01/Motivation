using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Messages.DeductionMessages;
using Motivation.HRWeb.Components.Pages.ExtraPartsAndDeductions.Models;

namespace Motivation.HRWeb.Components.Pages.ExtraPartsAndDeductions.ViewModels;

public class EditExtraPartModalViewModel
{
    private readonly IMediator _mediator;

    public EditExtraPartModalViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public ExtraPartFormModel Form { get; set; } = new();

    public void Initialize(ExtraPartDto? currentExtraPart, ExtraPartAndDeductionFilterModel filter)
    {
        if (currentExtraPart is not null)
        {
            Form.Title = currentExtraPart.Title ?? "";
            Form.Value = currentExtraPart.Value;
            Form.ValidFrom = currentExtraPart.ValidFrom;
            Form.ValidTo = currentExtraPart.ValidTo;
        }
        else
        {
            Form.Title = "";
            Form.Value = 0;
            Form.ValidFrom = new DateTime(filter.Year, filter.Month, 1);
            Form.ValidTo = new DateTime(filter.Year, filter.Month, DateTime.DaysInMonth(filter.Year, filter.Month));
        }

        Form.ErrorMessage = null;
        Form.IsSaving = false;
    }

    public async Task<ExtraPartSaveResult?> SaveAsync(string employeeCode, string positionCode, ExtraPartDto? currentExtraPart)
    {
        if (currentExtraPart is null)
        {
            currentExtraPart = new ExtraPartDto(
                Code: null,
                Title: Form.Title,
                PositionCode: positionCode,
                Value: Form.Value,
                ValidFrom: Form.ValidFrom,
                ValidTo: Form.ValidTo
            );
        }
        else
        {
            currentExtraPart = new ExtraPartDto(
                Code: currentExtraPart.Code,
                Title: Form.Title,
                PositionCode: positionCode,
                Value: Form.Value,
                ValidFrom: Form.ValidFrom,
                ValidTo: Form.ValidTo
            );
        }
        try
        {
            Form.IsSaving = true;
            Form.ErrorMessage = null;

            var result = await _mediator.Send(new ManageExtraPart.Request(employeeCode, currentExtraPart));

            if (!result.Ok)
            {
                Form.ErrorMessage = result.Error;
                return null;
            }
            else
            {
                return new ExtraPartSaveResult
                {
                    ExtraPart = result.Result,
                    EmployeeCode = employeeCode
                };
            }
        }
        catch (Exception exception)
        {
            Form.ErrorMessage = exception.Message;
            return null;
        }
        finally
        {
            Form.IsSaving = false;
        }
    }
}
