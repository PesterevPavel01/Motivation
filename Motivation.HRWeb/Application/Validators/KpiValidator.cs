using Calabonga.OperationResults;
using System.ComponentModel.DataAnnotations;

namespace Motivation.HRWeb.Application.Validators;

public class KpiValidator : IKpiValidator
{
    public Operation<bool, List<string>> ValidateNewKpi(PositionKpiItemModel newKpi, IEnumerable<PositionKpiItemModel> existingItems)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(newKpi.KpiCode))
        {
            errors.Add("Выберите KPI");
        }

        if (newKpi.OrderNumber <= 0)
        {
            errors.Add("Номер должен быть больше 0");
        }

        if (newKpi.Weight < 0)
        {
            errors.Add("Вес не может быть меньше 0");
        }

        if (newKpi.ValidFrom == default)
        {
            errors.Add("Укажите дату начала действия");
        }

        if (newKpi.ValidTo == default)
        {
            errors.Add("Укажите дату окончания действия");
        }

        if (newKpi.ValidTo != default && newKpi.ValidFrom != default && newKpi.ValidTo <= newKpi.ValidFrom)
        {
            errors.Add("Дата окончания должна быть позже даты начала");
        }

        if (existingItems.Any(x => x.OrderNumber == newKpi.OrderNumber))
        {
            errors.Add($"KPI с номером {newKpi.OrderNumber} уже существует");
        }

        if (errors.Any())
            return Operation.Error(errors);

        return true;
    }

    public Operation<bool, List<string>> ValidateEditItem(PositionKpiItemModel item, IEnumerable<PositionKpiItemModel> existingItems)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(item.KpiCode))
        {
            errors.Add("Выберите KPI");
        }

        if (item.OrderNumber <= 0)
        {
            errors.Add("Номер должен быть больше 0");
        }

        if (item.Weight < 0)
        {
            errors.Add("Вес не может быть меньше 0");
        }

        if (item.ValidFrom == default)
        {
            errors.Add("Укажите дату начала действия");
        }

        if (item.ValidTo == default)
        {
            errors.Add("Укажите дату окончания действия");
        }

        if (item.ValidTo != default && item.ValidFrom != default && item.ValidTo <= item.ValidFrom)
        {
            errors.Add("Дата окончания должна быть позже даты начала");
        }

        if (existingItems.Any(x => x.OrderNumber == item.OrderNumber && x != item))
        {
            errors.Add($"KPI с номером {item.OrderNumber} уже существует");
        }

        if (errors.Any())
            return Operation.Error(errors);

        return true;
    }
}