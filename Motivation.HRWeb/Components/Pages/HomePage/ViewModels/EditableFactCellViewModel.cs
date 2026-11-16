using Calabonga.OperationResults;
using Mediator;
using Microsoft.AspNetCore.Components;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Messages.KpiTarget;
using Motivation.HRWeb.Components.Pages.HomePage.Models;
using Motivation.HRWeb.Exceptions;

namespace Motivation.HRWeb.Components.Pages.HomePage.ViewModels;

public class EditableFactCellViewModel : IDisposable
{
    private readonly IMediator _mediator;
    private readonly NavigationManager _navigationManager;

    private EditableFactCellModel _model;

    private bool _isEditing;
    private decimal _editValue;
    private decimal _originalValue;
    private string? _errorMessage;

    public event Action? OnStateChanged;
    public event Action<bool>? OnSaveCompleted;

    public EditableFactCellViewModel(IMediator mediator, NavigationManager navigationManager)
    {
        _mediator = mediator;
        _navigationManager = navigationManager;
        _model = new EditableFactCellModel();
    }
    public decimal Value
    {
        get => _model.Value;
        set
        {
            if (_model.Value != value)
            {
                _model.Value = value;
                NotifyStateChanged();
            }
        }
    }
    public bool IsManual
    {
        get => _model.IsManual;
        set
        {
            if (_model.IsManual != value)
            {
                _model.IsManual = value;
                NotifyStateChanged();
            }
        }
    }

    public Guid? TargetId
    {
        get => _model.TargetId;
        set
        {
            if (_model.TargetId != value)
            {
                _model.TargetId = value;
                NotifyStateChanged();
            }
        }
    }

    public bool IsEditing
    {
        get => _isEditing;
        private set
        {
            if (_isEditing != value)
            {
                _isEditing = value;
                NotifyStateChanged();
            }
        }
    }

    public decimal EditValue
    {
        get => _editValue;
        set
        {
            if (_editValue != value)
            {
                _editValue = value;
                NotifyStateChanged();
            }
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                NotifyStateChanged();
            }
        }
    }

    public void Initialize(decimal value, bool isManual, Guid? targetId)
    {
        _model.Value = value;
        _model.IsManual = isManual;
        _model.TargetId = targetId;
        _editValue = value;
        _originalValue = value;
        IsEditing = false;
        ErrorMessage = null;
    }

    public void StartEdit()
    {
        if (!IsManual || !TargetId.HasValue)
            return;

        _originalValue = Value;
        EditValue = Value;
        IsEditing = true;
        ErrorMessage = null;
    }

    public async Task SaveAsync()
    {
        if (!Validate())
            return;

        try
        {
            var result = await SendSaveRequest();
            if (!result.Ok)
            {
                HandleError(result.Error);
                return;
            }

            HandleSuccess();
        }
        catch (RedirectToLoginException)
        {
            _navigationManager.NavigateTo("/authentication/login", forceLoad: true);
            OnSaveCompleted?.Invoke(false);
        }
        catch (Exception ex)
        {
            HandleError($"Ошибка при сохранении: {ex.Message}");
        }
    }

    public void Cancel()
    {
        EditValue = _originalValue;
        IsEditing = false;
        ErrorMessage = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();

    public void Dispose()
    {
        OnStateChanged = null;
        OnSaveCompleted = null;
    }

    private bool Validate()
    {
        if (!TargetId.HasValue)
        {
            ErrorMessage = "ID цели не указан";
            NotifyStateChanged();
            OnSaveCompleted?.Invoke(false);
            return false;
        }
        return true;
    }
    private async Task<Operation<bool, string>> SendSaveRequest()
    {
        var setResultModel = new SetEmployeeKpiResult(TargetId!.Value, EditValue);
        return await _mediator.Send(
            new SetKpiResultMessage.Request(setResultModel),
            default);
    }

    private void HandleSuccess()
    {
        Value = EditValue;
        IsEditing = false;
        ErrorMessage = null;
        NotifyStateChanged();
        OnSaveCompleted?.Invoke(true);
    }

    private void HandleError(string? error)
    {
        ErrorMessage = error ?? "Ошибка при сохранении";
        NotifyStateChanged();
        OnSaveCompleted?.Invoke(false);
    }
}
