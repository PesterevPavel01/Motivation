using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Messages.KpiMessages;
using Motivation.HRWeb.Application.Messages.PositionKpiMessages;
using Motivation.HRWeb.Application.Messages.PositionMessages;
using Motivation.HRWeb.Components.Pages.PositionsPage.Models;

namespace Motivation.HRWeb.Components.Pages.PositionsPage.ViewModels;

public class EditPositionModalViewModel : BasePositionModalViewModel
{
    private readonly IMediator _mediator;

    public EditPositionModalViewModel(IMediator mediator)
    {
        _mediator = mediator;
        AvailableKpis = new List<KpiDto>();
        LoadWeekTypes();
    }

    public List<KpiDto> AvailableKpis { get; set; }

    public async Task LoadAvailableKpisAsync()
    {
        try
        {
            var result = await _mediator.Send(new GetAllKpis.Request());
            if (result.Ok && result.Result != null)
            {
                AvailableKpis = result.Result.ToList();
                NotifyStateChanged();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotifyStateChanged();
        }
    }

    public void InitializeFromPosition(PositionDto position)
    {
        Code = position.Code;
        Title = position.Title;
        BaseSalary = position.BaseSalary;
        MotivationPart = position.MotivationPart;
        RecalculateToHours = position.RecalculateToHours;
        WorkWeekType = position.WorkWeekType;

        Kpis = position.Kpis?.Select(k => new PositionKpiItemModel
        {
            KpiCode = k.KpiCode,
            OrderNumber = k.OrderNumber,
            Weight = k.Weight,
            ValidFrom = k.ValidFrom,
            ValidTo = k.ValidTo,
            IsEditing = false,
            IsNew = false
        }).ToList() ?? new List<PositionKpiItemModel>();

        NotifyStateChanged();
    }

    private string GetKpiTitle(string kpiCode)
    {
        var kpi = AvailableKpis.FirstOrDefault(x => x.Code == kpiCode);
        return kpi?.Title ?? kpiCode;
    }

    public async Task<bool> UpdatePositionAsync()
    {
        if (!IsValid())
            return false;

        IsSaving = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            var editPosition = await UpdatePosition();
            if (!editPosition.Ok)
            {
                ErrorMessage = editPosition.Error;
                NotifyStateChanged();
                return false;
            }

            var createPositionKpis = await UpdatePositionKpis();
            if (!createPositionKpis.Ok)
            {
                ErrorMessage = createPositionKpis.Error;
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
            IsSaving = false;
            NotifyStateChanged();
        }
    }

    private bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            ErrorMessage = "Поле 'Название' обязательно для заполнения.";
            NotifyStateChanged();
            return false;
        }

        foreach (var kpi in Kpis)
        {
            if (!kpi.IsValid)
            {
                ErrorMessage = "Все поля KPI должны быть заполнены корректно.";
                NotifyStateChanged();
                return false;
            }
        }

        return true;
    }

    private async Task<Operation<bool, string>> UpdatePosition()
    {
        if (string.IsNullOrWhiteSpace(Code))
        {
            return Operation.Error("Code is null or white space!");
        }
        var dto = new PositionUpdateDto(
            Title,
            Code,
            BaseSalary,
            MotivationPart,
            RecalculateToHours,
            WorkWeekType
        );
        return await _mediator.Send(new UpdatePosition.Request(dto));
    }

    private async Task<Operation<bool, string>> UpdatePositionKpis()
    {
        if (!Kpis.Any())
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(Code))
        {
            return Operation.Error("Code is null or white space!");
        }
        var kpis = Kpis.Select(k => new AssignKpiToPositionDto(
                        k.KpiCode!,
                        k.OrderNumber,
                        k.Weight,
                        k.ValidFrom,
                        k.ValidTo
                    ));

        return await _mediator.Send(new AssignKpisToPosition.Request(Code, kpis));
    }
}