using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Messages.KpiMessages;
using Motivation.HRWeb.Application.Messages.PositionKpiMessages;
using Motivation.HRWeb.Application.Messages.PositionMessages;
using Motivation.HRWeb.Components.Pages.PositionsPage.Models;

namespace Motivation.HRWeb.Components.Pages.PositionsPage.ViewModels;

public class PositionModalViewModel
{
    private readonly IMediator _mediator;

    public PositionModalViewModel(IMediator mediator)
    {
        _mediator = mediator;
        Kpis = new List<PositionKpiItemModel>();
        AvailableKpis = new List<KpiDto>();
        WeekTypeOptions = WorkWeekTypeModel.GetWeekTypes().ToList();
    }

    public string Title { get; set; } = string.Empty;
    public string? Code { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal MotivationPart { get; set; }
    public bool RecalculateToHours { get; set; }
    public string WorkWeekType { get; set; } = string.Empty;
    public List<PositionKpiItemModel> Kpis { get; set; }
    public List<KpiDto> AvailableKpis { get; set; }
    public List<WorkWeekTypeModel> WeekTypeOptions { get; }

    public bool IsEditMode { get; private set; }
    public bool IsSaving { get; private set; }
    public string? ErrorMessage { get; private set; }

    public event Action? OnStateChanged;

    public void SetMode(bool isEditMode)
    {
        IsEditMode = isEditMode;
        NotifyStateChanged();
    }

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
            KpiTitle = k.KpiTitle,
            OrderNumber = k.OrderNumber,
            Weight = k.Weight,
            ValidFrom = k.ValidFrom,
            ValidTo = k.ValidTo,
            IsEditing = false,
            IsNew = false
        }).ToList() ?? new List<PositionKpiItemModel>();

        NotifyStateChanged();
    }

    public void Reset()
    {
        Code = null;
        Title = string.Empty;
        BaseSalary = 0;
        MotivationPart = 0;
        RecalculateToHours = false;
        WorkWeekType = string.Empty;
        Kpis = new List<PositionKpiItemModel>();
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public async Task<bool> SavePositionAsync()
    {
        if (!IsValid())
            return false;

        IsSaving = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            Operation<bool, string> result;

            if (IsEditMode)
            {
                var editPosition = await UpdatePosition();
                if (!editPosition.Ok)
                {
                    ErrorMessage = editPosition.Error;
                    NotifyStateChanged();
                    return false;
                }

                var createPositionKpis = await AssignKpisToPosition();
                if (!createPositionKpis.Ok)
                {
                    ErrorMessage = createPositionKpis.Error;
                    NotifyStateChanged();
                    return false;
                }
            }
            else
            {
                var createPosition = await CreatePosition();
                if (!createPosition.Ok)
                {
                    ErrorMessage = createPosition.Error;
                    NotifyStateChanged();
                    return false;
                }
                Code = createPosition.Result.Code;

                var createPositionKpis = await AssignKpisToPosition();
                if (!createPositionKpis.Ok)
                {
                    ErrorMessage = createPositionKpis.Error;
                    NotifyStateChanged();
                    return false;
                }
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

    public void OnKpisChanged()
    {
        NotifyStateChanged();
    }

    private bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            ErrorMessage = "Поле 'Название' обязательно для заполнения.";
            NotifyStateChanged();
            return false;
        }

        if (IsEditMode && string.IsNullOrWhiteSpace(Code))
        {
            ErrorMessage = "Код должности не найден.";
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

    private void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }

    private async Task<Operation<PositionDto, string>> CreatePosition()
    {
        var dto = new PositionCreateDto(
            Title,
            null,
            BaseSalary,
            MotivationPart,
            RecalculateToHours,
            WorkWeekType
        );
        return await _mediator.Send(new CreatePosition.Request(dto));
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

    private async Task<Operation<bool, string>> AssignKpisToPosition()
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