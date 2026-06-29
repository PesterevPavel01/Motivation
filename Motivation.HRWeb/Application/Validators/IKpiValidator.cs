using Calabonga.OperationResults;
using System.ComponentModel.DataAnnotations;

namespace Motivation.HRWeb.Application.Validators;

public interface IKpiValidator
{
    Operation<bool, List<string>> ValidateNewKpi(PositionKpiItemModel newKpi, IEnumerable<PositionKpiItemModel> existingItems);
    Operation<bool, List<string>> ValidateEditItem(PositionKpiItemModel item, IEnumerable<PositionKpiItemModel> existingItems);
}