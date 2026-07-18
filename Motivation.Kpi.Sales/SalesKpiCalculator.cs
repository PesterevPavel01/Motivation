using Calabonga.OperationResults;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Contracts.Interfaces;
using Motivation.Domain.Enum;

namespace Motivation.KpiSalesCalculator;

public sealed class SalesKpiCalculator : ICalculator
{
    public KpiType KpiType => KpiType.Sales;

    public async Task<Operation<KpiCalculationTaskResult, string>> CalculateKpiAsync(KpiCalculationTask model) 
    {
        if (model is null)
        {
            return Operation.Error("KpiCalculationTask not found!");
        }

        var days = (model.PeriodTo - model.PeriodFrom).Days + 1;

        return new KpiCalculationTaskResult 
        {
            PositionKpiId = model.PositionKpiId,
            PeriodFrom = model.PeriodFrom,
            PeriodTo = model.PeriodTo,
            Target = model.Target,
            Value = 2000 * days,
            Achievement = 2000 * days / model.Target * 100
        };
    }
}
