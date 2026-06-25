using Motivation.Contracts.Dto;

namespace Motivation.HRWeb.Components.Pages.ExtraPartsAndDeductions.Models;

public class ExtraPartSaveResult
{
    public ExtraPartDto? ExtraPart { get; set; }
    public string? EmployeeCode { get; set; }
}
