using Calabonga.OperationResults;
using Microsoft.Extensions.Logging;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.BasketComplexityKpiCalculator.Interfaces;
using Motivation.BasketComplexityKpiCalculator.Mappers;
using Motivation.Contracts.Dto;
using Motivation.KpiCalculator.Core.Enums;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.BasketComplexityKpiCalculator.Strategies;

public class ComplexityCalculationStrategy : ICalculationStrategy<CustomerTransactionDto>
{
    private readonly IValidator<IEnumerable<CustomerTransactionDto>> _managerValidator;
    private readonly ComplexityItemsMapper _complexityItemsMapper;
    private readonly IComplexityCalculateProcessor _complexityCalculateProcessor;
    private readonly ILogger<ComplexityCalculationStrategy> _logger;
    private readonly IComplexityCalculationConfigurations _configurations;

    public CalculationStrategyType StrategyType => CalculationStrategyType.Complexity;

    public ComplexityCalculationStrategy(

        IValidator<IEnumerable<CustomerTransactionDto>> managerValidator,
        ComplexityItemsMapper itemsBuilder,
        IComplexityCalculateProcessor complexityCalculateProcessor,
        ILogger<ComplexityCalculationStrategy> logger,
        IComplexityCalculationConfigurations configurations)
    {
        _managerValidator = managerValidator;
        _complexityItemsMapper = itemsBuilder;
        _complexityCalculateProcessor = complexityCalculateProcessor;
        _logger = logger;
        _configurations = configurations;
    }

    public Operation<decimal, string> Calculate(
        IEnumerable<CustomerTransactionDto> transactions,
        MeasurementUnitDto measurementUnit)
    {
        if (transactions is null || !transactions.Any())
            return 0;

        var validationResult = _managerValidator.Validate(transactions);
        if (!validationResult.Ok)
            return Operation.Error(validationResult.Error);

        var itemsResult = _complexityItemsMapper.FromTransactionModels(transactions, _configurations);
        if (!itemsResult.Ok)
            return Operation.Error(itemsResult.Error);

        var result = _complexityCalculateProcessor.Process(itemsResult.Result, _configurations);

        _logger.LogInformation("Complexity calculation completed. Successful clients: {Count}", result);
        return result;
    }
}