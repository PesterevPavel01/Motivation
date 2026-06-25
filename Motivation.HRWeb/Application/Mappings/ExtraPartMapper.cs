using Motivation.Contracts.Dto;
using Motivation.HRWeb.Components.Pages.Employees.Models;
using System.ComponentModel.DataAnnotations;

namespace Motivation.HRWeb.Application.Mappings;

public static class ExtraPartMapper
{
    public static CreateExtraPartDto ToCreateDto(this ExtraPartDto model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        return new CreateExtraPartDto(
            Code: model.Code,
            Title: model.Title,
            PositionCode: model.PositionCode,
            Value: model.Value,
            ValidFrom: model.ValidFrom,
            ValidTo:model.ValidTo
        );
    }
}