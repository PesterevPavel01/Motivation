using Calabonga.OperationResults;

namespace Motivation.AnalyticsCore.Contracts.Interfaces;

public interface IEntityServices<TDto>
{
    Task<Operation<bool, string>> CreateOrEditAsync(Dictionary<string, TDto> models, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken);
    Task<Operation<bool, string>> DeleteAsync(DateTime dateStart, DateTime dateEnd);
}
