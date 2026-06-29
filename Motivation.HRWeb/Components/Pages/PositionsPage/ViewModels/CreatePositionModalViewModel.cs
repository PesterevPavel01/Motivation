using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Messages.KpiMessages;
using Motivation.HRWeb.Application.Messages.PositionKpiMessages;
using Motivation.HRWeb.Application.Messages.PositionMessages;

namespace Motivation.HRWeb.Components.Pages.PositionsPage.ViewModels;

public class CreatePositionModalViewModel : BasePositionModalViewModel
{
    private readonly IMediator _mediator;

    public CreatePositionModalViewModel(IMediator mediator)
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

    public async Task<bool> CreatePositionAsync()
    {
        if (!IsValid())
            return false;

        IsSaving = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            var createPosition = await CreatePosition();
            if (!createPosition.Ok)
            {
                ErrorMessage = createPosition.Error;
                NotifyStateChanged();
                return false;
            }
            Code = createPosition.Result.Code;

            var createPositionKpis = await CreatePositionKpis();
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

    private async Task<Operation<PositionDto, string>> CreatePosition() 
    {
        var dto = new PositionCreateDto(
            Title,
            Code ?? string.Empty,
            BaseSalary,
            MotivationPart,
            RecalculateToHours,
            WorkWeekType
        );
        return await _mediator.Send(new CreatePosition.Request(dto));
    }

    private async Task<Operation<bool, string>> CreatePositionKpis()
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