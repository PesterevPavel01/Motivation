using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;

namespace Motivation.Application.Mappings;

public static class EmployeeMapper
{
    public static EmployeeDto MapToDto(this Employee source)
    {
        if (source is null || source.TargetHistory is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var model = new EmployeeDto {
            Code = source.Code.Value,
            FirstName = source.FirstName.Value,
            SecondName = source.SecondName?.Value,
            LastName = source.LastName.Value,
            FullName = source.FullName.Value,
            Positions = source.EmployeePositions
                .Select(x => x.MapToDto())
                .ToList()
        };

        FilterTargetsByEmployee(model, source.Code.Value);
        return model;
    }

    private static void FilterTargetsByEmployee(EmployeeDto employeeDto, string employeeCode)
    {
        foreach (var positionDto in employeeDto.Positions)
        {
            if (positionDto?.Position?.Kpis is null)
                continue;

            foreach (var kpiDto in positionDto.Position.Kpis)
            {
                if (kpiDto.Targets is null || !kpiDto.Targets.Any())
                    continue;

                var filtered = new List<KpiTargetDto>(kpiDto.Targets.Count);

                foreach (var target in kpiDto.Targets)
                {
                    if (target.EmployeeCode == employeeCode)
                    {
                        filtered.Add(target);
                    }
                }

                kpiDto.Targets = filtered;
            }
        }
    }
}
