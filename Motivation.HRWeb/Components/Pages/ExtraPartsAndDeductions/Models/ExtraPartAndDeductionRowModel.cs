using Motivation.Contracts.Dto;

namespace Motivation.HRWeb.Components.Pages.ExtraPartsAndDeductions.Models;

public class ExtraPartAndDeductionRowModel
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeFullName { get; set; } = string.Empty;
    public string PositionCode { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;

    public string? DeductionCode { get; set; }  
    public decimal? DeductionValue { get; set; }   
    public decimal DeductionEditingValue { get; set; } 
    public bool IsDeductionEditing { get; set; }    
    public bool IsDeductionSaving { get; set; }    

    public ExtraPartDto? CurrentExtraPart { get; set; } 
    public decimal? ExtraPartValue { get; set; }    
}