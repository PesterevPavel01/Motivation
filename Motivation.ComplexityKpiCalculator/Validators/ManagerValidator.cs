using Calabonga.OperationResults;
using Microsoft.Extensions.Logging;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.BasketComplexityKpiCalculator.Interfaces;

namespace Motivation.BasketComplexityKpiCalculator.Validators;

public class ManagerValidator : IValidator<IEnumerable<CustomerTransactionDto>>
{
    private readonly ILogger<ManagerValidator> _logger;

    public ManagerValidator(ILogger<ManagerValidator> logger)
    {
        _logger = logger;
    }

    public Operation<bool, string> Validate(IEnumerable<CustomerTransactionDto> transactions)
    {
        var managers = transactions
            .Select(t => t.Customer?.Manager)
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .Distinct()
            .ToList();

        return managers.Count switch
        {
            0 => Operation.Error("No manager found in transactions!"),
            > 1 => Operation.Error($"Multiple managers found: {string.Join(", ", managers)}!"),
            _ => true
        };
    }
}