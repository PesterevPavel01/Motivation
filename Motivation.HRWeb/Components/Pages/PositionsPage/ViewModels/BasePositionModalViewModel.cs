using Motivation.HRWeb.Components.Pages.PositionsPage.Models;

namespace Motivation.HRWeb.Components.Pages.PositionsPage.ViewModels;

public abstract class BasePositionModalViewModel
{
    public string Title { get; set; } = string.Empty;
    public string? Code { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal MotivationPart { get; set; }
    public bool RecalculateToHours { get; set; }
    public string WorkWeekType { get; set; } = string.Empty;
    public List<PositionKpiItemModel> Kpis { get; set; } = new();

    public List<WorkWeekTypeModel> WeekTypeOptions { get; set; } = new();

    public bool IsSaving { get; protected set; }
    public string? ErrorMessage { get; protected set; }

    public event Action? OnStateChanged;

    protected void NotifyStateChanged() => OnStateChanged?.Invoke();

    protected void LoadWeekTypes()
    {
        WeekTypeOptions = WorkWeekTypeModel.GetWeekTypes().ToList();
    }
}