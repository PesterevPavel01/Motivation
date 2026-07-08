using Calabonga.OperationResults;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Contracts.Interfaces;
using Motivation.Domain.Enum;

namespace Motivation.KpiSalesCalculator;

public sealed class SalesKpiCalculator : ICalculator
{
    public KpiType KpiType => KpiType.Sales;

    public async Task<Operation<decimal, string>> CalculateKpiAsync(CalculateEmployeeKpiDto model) 
    {
        var kpi = model.Employee.Positions
            .FirstOrDefault(x => x.Position?.Code == model.PositionCode)?
            .Position?.Kpis?.FirstOrDefault(x => x.KpiCode == model.KpiCode);

        if (kpi is null || kpi.Kpi is null)
        {
            return Operation.Error("Kpi not found!");
        }

        var days = (model.PeriodTo - model.PeriodFrom).Days + 1;
        return kpi.Kpi.CustomerResponsibleFilter.Any() ? 2000 * days : 23000 * days;
    }
}
