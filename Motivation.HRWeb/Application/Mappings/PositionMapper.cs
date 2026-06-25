using Motivation.Contracts.Dto;
using Motivation.Domain.Enum;
using Motivation.HRWeb.Components.Pages.PositionsPage.ViewModels;

namespace Motivation.HRWeb.Application.Mappings;

public static class PositionMapper
{
    public static EditPositionViewModel ToUpdateViewModel(this PositionDto dto)
    {
        return new EditPositionViewModel
        {
            Title = dto.Title,
            Code = dto.Code,
            BaseSalary = dto.BaseSalary,
            MotivationPart = dto.MotivationPart,
            RecalculateToHours = dto.RecalculateToHours,
            WorkWeekType = dto.WorkWeekType
        };
    }
    public static PositionCreateDto ToCreateDto(this CreatePositionViewModel model)
    {
        return new PositionCreateDto(
            Title: model.Title,
            Code: model.Code,
            BaseSalary: model.BaseSalary,
            MotivationPart: model.MotivationPart,
            RecalculateToHours: model.RecalculateToHours,
            WorkWeekType: model.WorkWeekType
        );
    }

    public static CreatePositionViewModel ToViewModel(this PositionCreateDto dto)
    {
        return new CreatePositionViewModel
        {
            Title = dto.Title,
            Code = dto.Code,
            BaseSalary = dto.BaseSalary,
            MotivationPart = dto.MotivationPart,
            RecalculateToHours = dto.RecalculateToHours,
            WorkWeekType = dto.WorkWeekType
        };
    }

    public static PositionUpdateDto ToUpdateDto(this EditPositionViewModel model)
    {
        return new PositionUpdateDto(
            Title: model.Title,
            Code: model.Code,
            BaseSalary: model.BaseSalary,
            MotivationPart: model.MotivationPart,
            RecalculateToHours: model.RecalculateToHours,
            WorkWeekType: model.WorkWeekType
        );
    }

    public static EditPositionViewModel ToViewModel(this PositionUpdateDto dto)
    {
        return new EditPositionViewModel
        {
            Title = dto.Title,
            Code = dto.Code,
            BaseSalary = dto.BaseSalary,
            MotivationPart = dto.MotivationPart,
            RecalculateToHours = dto.RecalculateToHours,
            WorkWeekType = dto.WorkWeekType
        };
    }
}