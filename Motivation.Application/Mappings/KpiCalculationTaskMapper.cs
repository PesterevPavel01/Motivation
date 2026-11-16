using Motivation.Contracts.Dto;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Contracts.Models;
using Motivation.Domain.Entities;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.Enum;

namespace Motivation.Application.Mappings;

public static class KpiCalculationTaskMapper
{
    public static KpiCalculationTask? MapToCalculationTaskDto(
        this PositionKpi positionKpi,
        DateTime periodFrom,
        DateTime periodTo,
        Employee employee,
        Position position,
        Guid TargetId)
    {
        var kpi = positionKpi?.Kpi;

        if (positionKpi is null || kpi is null || employee is null || position is null)
            return null;

        var filters = kpi?.Filters ?? [];

        var kpiFilterModels = filters
            .Select(f =>
                new KpiFilterDto(
                    f.Title.Value != "default" ? f.Title.Value : employee.FullName.Value, f.Include, f.KpiFilterType))
            .ToList();

        var target = positionKpi.TargetHistory
            .FirstOrDefault(x => 
                x.ValidFrom >= periodFrom
                && x.ValidTo <= periodTo
                && x.Id == TargetId);

        if (target is null)
        {
            throw new ArgumentException("Target is null!");
        }

        var manualResiult = kpi?.KpiType == KpiType.Manual && target.KpiFact is not null?
            new ManualResultDto(target.KpiFact.Fact.Value)
            : null;

        return new KpiCalculationTask
        {
            PeriodFrom = periodFrom,
            PeriodTo = periodTo,
            PositionKpiId = positionKpi.Id,
            EmployeeId = employee.Id,
            CalculationType = kpi!.CalculationType.ToString(),
            Target = target.Target.Value,
            MotivationPart = position.MotivationPart.MotivationPartValue.Value,
            Weight = positionKpi.Weight.Value,

            MeasurementUnit = new MeasurementUnitDto(
                kpi.MeasurementUnit.Code.Value,
                kpi.MeasurementUnit.Title.Value
            ),

            KpiType = kpi.KpiType.ToString(),

            KpiFilters = kpiFilterModels,
            ManualResult = manualResiult
        };
    }
}
