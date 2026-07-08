using Calabonga.OperationResults;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Domain.Enum;

namespace Motivation.Contracts.Interfaces;

public interface ICalculator
{
    KpiType KpiType { get; }
    Task<Operation<decimal, string>> CalculateKpiAsync(CalculateEmployeeKpiDto model);
}
