using Calabonga.OperationResults;
using Microsoft.Extensions.Logging;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Contracts.Interfaces;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiCalculator.Core;

public abstract class BaseKpiCalculator<TDto> : ICalculator
    where TDto : class
{
    protected readonly IKpiQueryBuilder<TDto> _queryBuilder;
    protected readonly ILogger<BaseKpiCalculator<TDto>> _logger;

    protected BaseKpiCalculator(
        IKpiQueryBuilder<TDto> queryBuilder,
        ILogger<BaseKpiCalculator<TDto>> logger)
    {
        _queryBuilder = queryBuilder;
        _logger = logger;
    }

    public abstract KpiType KpiType { get; }
    public abstract ICalculationStrategy<TDto>? GetStrategy(KpiCalculationTask task);

    public async Task<Operation<KpiCalculationTaskResult, string>> CalculateKpiAsync(
        KpiCalculationTask task,
        CancellationToken cancellationToken = default)
    {
        if (task is null)
            return Operation.Error("KpiCalculationTask not found!");

        try
        {
            var queryResult = await BuildQueryAsync(task, cancellationToken);
            if (queryResult is null)
                return Operation.Error("Query result is null");

            var strategy = GetStrategy(task);
            if (strategy is null)
                return Operation.Error("Strategy not found!"); 

            var actualValue = strategy.Calculate(queryResult.Transactions, task.MeasurementUnit);
            if (!actualValue.Ok)
                return Operation.Error(actualValue.Error);

            var achievement = CalculateAchievement(actualValue.Result, task.Target, task.CalculationType);
            if (!achievement.Ok)
                return Operation.Error(achievement.Error);

            var bonusPercent = CalculateBonusPercent(achievement.Result, task.CalculationType);
            if (!bonusPercent.Ok)
                return Operation.Error(bonusPercent.Error);

            var bonusAmount = task.CalculationType?.ToLower() == "progressive"
                ? CalculateProgressiveBonusAmount(actualValue.Result, task.Target)
                : CalculateDefaultBonusAmount(bonusPercent.Result, task.Weight, task.MotivationPart);

            if (!bonusAmount.Ok)
                return Operation.Error(bonusAmount.Error);

            return new KpiCalculationTaskResult
            {
                PeriodFrom = task.PeriodFrom,
                PeriodTo = task.PeriodTo,
                PositionKpiId = task.PositionKpiId,
                EmployeeId = task.EmployeeId,
                Target = task.Target,
                Value = Math.Round(actualValue.Result, 2),
                Achievement = Math.Round(achievement.Result, 2),
                BonusPercent = Math.Round(bonusPercent.Result, 2),
                BonusAmount = Math.Round(bonusAmount.Result, 2)
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error calculating {KpiType} KPI for Employee {EmployeeId}", KpiType, task.EmployeeId);
            return Operation.Error($"Error calculating {KpiType} KPI for Employee {task.EmployeeId}. Exception: {exception.Message}");
        }
    }

    protected virtual async Task<KpiQueryResult<TDto>?> BuildQueryAsync(
        KpiCalculationTask task,
        CancellationToken cancellationToken)
    {
        return await _queryBuilder
            .SetPeriod(task.PeriodFrom, task.PeriodTo)
            .InitializeFilters(task.KpiFilters)
            .BuildAsync(cancellationToken);
    }

    protected Operation<decimal, string> CalculateAchievement(decimal actualValue, decimal target, string calculationType)
        => calculationType?.ToLower() switch
        {
            "standard" => CalculateStandardAchievement(actualValue, target),
            "reversed" => CalculateReversedAchievement(actualValue, target),
            "progressive" => CalculateProgressiveAchievement(actualValue, target),
            _ => 0
        };

    private Operation<decimal, string> CalculateStandardAchievement(decimal actualValue, decimal target)
    {
        if (target == 0) return Operation.Error("Target is 0");
        return Math.Round((actualValue / target) * 100, 2);
    }

    private Operation<decimal, string> CalculateReversedAchievement(decimal actualValue, decimal target)
    {
        if (target == 0) return 0;
        return Math.Round((target / actualValue) * 100, 2);
    }

    private Operation<decimal, string> CalculateProgressiveAchievement(decimal actualValue, decimal target)
    {
        if (target == 0) return Operation.Error("Target is 0");
        return Math.Round(target / 100 * actualValue, 2);
    }

    protected Operation<decimal, string> CalculateBonusPercent(decimal achievement, string calculationType)
        => calculationType?.ToLower() == "progressive"
            ? 100
            : CalculateDefaultBonusPercent(achievement);

    private Operation<decimal, string> CalculateDefaultBonusPercent(decimal achievement)
    {
        if (achievement < 90) return 0;
        if (achievement >= 100) return 100;
        return 50 + 5 * (achievement - 90);
    }

    protected Operation<decimal, string> CalculateProgressiveBonusAmount(decimal actualValue, decimal target)
    {
        if (target == 0) return Operation.Error("Target is 0");
        return Math.Round(target / 100 * actualValue, 2);
    }

    protected Operation<decimal, string> CalculateDefaultBonusAmount(decimal bonusPercent, decimal weight, decimal motivationPart)
    {
        if (weight == 0) return Operation.Error("Weight is 0");
        if (motivationPart == 0) return Operation.Error("MotivationPart is 0");
        return Math.Round(bonusPercent / 100 * weight * motivationPart, 2);
    }
}