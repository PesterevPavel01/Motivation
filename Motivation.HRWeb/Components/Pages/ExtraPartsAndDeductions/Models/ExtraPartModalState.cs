using Motivation.Contracts.Dto;

namespace Motivation.HRWeb.Components.Pages.ExtraPartsAndDeductions.Models;

public class ExtraPartModalState
{
    public bool IsOpen { get; set; }
    public string EmployeeCode { get; set; } = "";
    public string EmployeeFullName { get; set; } = "";
    public string PositionCode { get; set; } = "";
    public ExtraPartDto? CurrentExtraPart { get; set; }
    public ExtraPartAndDeductionFilterModel Filter { get; set; } = new();
}
