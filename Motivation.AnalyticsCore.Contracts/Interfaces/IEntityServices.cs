using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.AnalyticsCore.Contracts.Interfaces;

public interface IEntityServices<TEntity, TDto>
    where TEntity : class 
    where TDto : class
{
    Task<Operation<List<TEntity>, string>> CreateOrEditAsync(List<TDto> models);
    Task<Operation<bool, string>> DeleteAsync(DateTime dateStart, DateTime dateEnd);
}
