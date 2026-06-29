using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Messages.PositionMessages;

namespace Motivation.HRWeb.Components.Pages.PositionsPage.ViewModels;

public class DeletePositionModalViewModel
{
    private readonly IMediator _mediator;

    public DeletePositionModalViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public string? PositionCode { get; private set; }
    public string? PositionTitle { get; private set; }
    public bool IsDeleting { get; private set; }
    public string? ErrorMessage { get; private set; }

    public event Action? OnStateChanged;

    public void InitializeFromPosition(PositionDto position)
    {
        PositionCode = position.Code;
        PositionTitle = position.Title;
        NotifyStateChanged();
    }

    public async Task<bool> DeletePositionAsync()
    {
        if (string.IsNullOrEmpty(PositionCode))
            return false;

        IsDeleting = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            var result = await _mediator.Send(new DeletePosition.Request(PositionCode));

            if (!result.Ok)
            {
                ErrorMessage = result.Error ?? "Ошибка удаления должности.";
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
            IsDeleting = false;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }
}