using Motivation.Contracts.Dto;
using Motivation.Contracts.Dto.CalculateDto;
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

        var filters = kpi?.Filters ?? Enumerable.Empty<KpiFilter>();

        var target = positionKpi.TargetHistory
            .FirstOrDefault(x => 
                x.ValidFrom >= periodFrom
                && x.ValidTo <= periodTo
                && x.Id == TargetId);

        if (target is null)
        {
            throw new ArgumentException("Target is null!");
        }

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

            CustomerFilter = filters
                .Where(f => f.KpiFilterType == KpiFilterType.Customer)
                .Select(f => new KpiFilterDto(f.Title.Value, f.Include)),

            SupplierFilter = filters
                .Where(f => f.KpiFilterType == KpiFilterType.Supplier)
                .Select(f => new KpiFilterDto(f.Title.Value, f.Include)),

            ProductFilter = filters
                .Where(f => f.KpiFilterType == KpiFilterType.Product)
                .Select(f => new KpiFilterDto(f.Title.Value, f.Include)),

            ProductStatusFilter = filters
                .Where(f => f.KpiFilterType == KpiFilterType.ProductStatus)
                .Select(f => new KpiFilterDto(f.Title.Value, f.Include)),

            CompanyFilter = filters
                .Where(f => f.KpiFilterType == KpiFilterType.Company)
                .Select(f => new KpiFilterDto(f.Title.Value, f.Include))
                .ToArray(),

            CustomerResponsibleFilter = filters
                .Where(f => f.KpiFilterType == KpiFilterType.CustomerResponsible)
                .Select(x => new KpiFilterDto(employee.FullName.Value, x.Include)),

            SalesResponsibleFilter = filters
                .Where(f => f.KpiFilterType == KpiFilterType.SalesResponsible)
                .Select(f => new KpiFilterDto(employee.FullName.Value, f.Include)),

            ProductResponsibleFilter = filters
                .Where(f => f.KpiFilterType == KpiFilterType.ProductResponsible)
                .Select(f => new KpiFilterDto(employee.FullName.Value, f.Include))
        };
    }
}
