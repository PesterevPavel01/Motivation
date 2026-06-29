using Motivation.Contracts.Dto;
using Motivation.Domain.Enum;
using Motivation.HRWeb.Components.Pages.PositionsPage.Models;

namespace Motivation.HRWeb.Application.Mappings;

public static class PositionMapper
{
    public static EditPositionModel ToUpdateViewModel(this PositionDto dto)
    {
        return new EditPositionModel
        {
            Title = dto.Title,
            Code = dto.Code,
            BaseSalary = dto.BaseSalary,
            MotivationPart = dto.MotivationPart,
            RecalculateToHours = dto.RecalculateToHours,
            WorkWeekType = dto.WorkWeekType
        };
    }
    public static PositionCreateDto ToCreateDto(this CreatePositionModel model)
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

    public static CreatePositionModel ToViewModel(this PositionCreateDto dto)
    {
        return new CreatePositionModel
        {
            Title = dto.Title,
            Code = dto.Code,
            BaseSalary = dto.BaseSalary,
            MotivationPart = dto.MotivationPart,
            RecalculateToHours = dto.RecalculateToHours,
            WorkWeekType = dto.WorkWeekType
        };
    }

    public static PositionUpdateDto ToUpdateDto(this EditPositionModel model)
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

    public static EditPositionModel ToViewModel(this PositionUpdateDto dto)
    {
        return new EditPositionModel
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