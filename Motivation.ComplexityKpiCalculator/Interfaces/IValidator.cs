using Calabonga.OperationResults;

namespace Motivation.BasketComplexityKpiCalculator.Interfaces;

public interface IValidator<TInput>
{
    Operation<bool, string> Validate(TInput input);
}
