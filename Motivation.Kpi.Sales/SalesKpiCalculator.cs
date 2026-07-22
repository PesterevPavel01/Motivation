using Calabonga.OperationResults;
using Microsoft.Extensions.Logging;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Contracts.Interfaces;
using Motivation.Domain.Enum;
using Motivation.KpiSalesCalculator.Interfaces;

namespace Motivation.KpiSalesCalculator;

public sealed class SalesKpiCalculator : ICalculator
{
    private readonly IKpiQueryBuilder _queryBuilder;
    private readonly ICalculationStrategyFactory _strategyFactory;
    private readonly ILogger<SalesKpiCalculator> _logger;

    public KpiType KpiType => KpiType.Sales;

    public SalesKpiCalculator(
        IKpiQueryBuilder queryBuilder,
        ICalculationStrategyFactory strategyFactory,
        ILogger<SalesKpiCalculator> logger)
    {
        _queryBuilder = queryBuilder;
        _strategyFactory = strategyFactory;
        _logger = logger;
    }

    public async Task<Operation<KpiCalculationTaskResult, string>> CalculateKpiAsync(
        KpiCalculationTask task, 
        CancellationToken cancellationToken = default) 
    {
        if (task is null)
        {
            return Operation.Error("KpiCalculationTask not found!");
        }

        try
        {
            _logger.LogInformation("Starting KPI calculation for Employee {EmployeeId}, Period {PeriodFrom} - {PeriodTo}",
                task.EmployeeId, task.PeriodFrom, task.PeriodTo);

            KpiQueryResult? queryResult;
            try
            {
                queryResult = await _queryBuilder
                    .SetPeriod(task.PeriodFrom, task.PeriodTo)
                    .ApplyCustomerFilter(task.CustomerFilter)
                    .ApplyProductSupplierFilter(task.SupplierFilter)
                    .ApplyProductFilter(task.ProductFilter)
                    .ApplyProductStatusFilter(task.ProductStatusFilter)
                    .ApplyCompanyFilter(task.CompanyFilter)
                    .ApplyResponsibleFilters(
                        task.CustomerResponsibleFilter,
                        task.SalesResponsibleFilter,
                        task.ProductResponsibleFilter)
                    .BuildAsync(cancellationToken);
            }
            catch (Exception exception) 
            {
                _logger.LogError(exception, "Failed to build KPI query");
                return Operation.Error(exception.Message);
            }

            if (queryResult is null)
            {
                return Operation.Error("Query result is null");
            }

            var strategy = _strategyFactory.GetStrategy(task.MeasurementUnit);

            var actualValue = strategy.Calculate(queryResult.Transactions, task.MeasurementUnit);

            var achievement = CalculateAchievement(actualValue, task.Target, task.CalculationType);
            if (!achievement.Ok)
            {
                return Operation.Error(achievement.Error);
            }

            var bonusPercent = CalculateBonusPercent(achievement.Result, task.CalculationType);
            if (!bonusPercent.Ok)
            {
                return Operation.Error(bonusPercent.Error);
            }

            var bomusAmount = task.CalculationType?.ToLower() == "progressive" ?
                CalculateProgressiveBonusAmount(actualValue, task.Target)
                : CalculateDefaultBonusAmount(bonusPercent.Result, task.Weight, task.MotivationPart);
            if (!bomusAmount.Ok)
            {
                return Operation.Error(bomusAmount.Error);
            }

            return new KpiCalculationTaskResult
            {
                PeriodFrom = task.PeriodFrom,
                PeriodTo = task.PeriodTo,
                PositionKpiId = task.PositionKpiId,
                EmployeeId = task.EmployeeId,
                Target = task.Target,
                Value = Math.Round(actualValue, 2),
                Achievement = Math.Round(achievement.Result,2),
                BonusPercent = Math.Round(bonusPercent.Result, 2),
                BonusAmount = Math.Round(bomusAmount.Result, 2),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating KPI for Employee {EmployeeId}", task.EmployeeId);
            throw;
        }
    }

    private Operation<decimal, string> CalculateAchievement(decimal actualValue, decimal target, string calculationType)
        =>
        calculationType?.ToLower() switch
        {
            "standard" => CalculateStandardAchievement(actualValue, target),
            "reversed" => CalculateReversedAchievement(actualValue, target),
            "progressive" => CalculateProgressiveAchievement(actualValue, target),
            _ => 0
        };

    private Operation<decimal, string> CalculateStandardAchievement(decimal actualValue, decimal target)
    {
        if (target == 0)
            return Operation.Error("Target is 0");

        return Math.Round((actualValue / target) * 100, 2);
    }
    private Operation<decimal, string> CalculateReversedAchievement(decimal actualValue, decimal target)
    {
        if (target == 0)
            return 0;

        return Math.Round((target/actualValue) * 100, 2);
    }
    private Operation<decimal, string> CalculateProgressiveAchievement(decimal actualValue, decimal target)
    {
        if (target == 0)
            return Operation.Error("Target is 0");

        return Math.Round(target/100 * actualValue, 2);
    }

    private Operation<decimal, string> CalculateBonusPercent(decimal achievement, string calculationType)
    {
        return calculationType?.ToLower() switch
        {
            "progressive" => 100,
            _ => CalculateDefaultBonusPercent(achievement)
        };
    }

    private Operation<decimal, string> CalculateDefaultBonusPercent(decimal achievement)
    {
        if(achievement < 90)
            return 0;

        if (achievement >= 100)
            return 100;

        return 50 + 5 * (achievement - 90);
    }

    private Operation<decimal, string> CalculateProgressiveBonusAmount(decimal actualValue, decimal target)
    {
        if (target == 0)
            return Operation.Error("Target is 0");

        return Math.Round(target/100 * actualValue, 2);
    }

    private Operation<decimal, string> CalculateDefaultBonusAmount(decimal bonusPercent, decimal weight, decimal motivationPart)
    {
        if (weight == 0)
            return Operation.Error("Weight is 0");
        if (motivationPart == 0)
            return Operation.Error("MotivationPart is 0");

        return Math.Round(bonusPercent / 100 * weight * motivationPart, 2);
    }
}
