using Motivation.AnalyticsCore.Domain.Enum;

namespace Motivation.AnalyticsCore.Application.Mappers;

public static class WarehouseTransactionTypeMapper
{
    private static readonly Dictionary<WarehouseTransactionType, string> RussianNames = new()
    {
        { WarehouseTransactionType.Return, "Возврат" },
        { WarehouseTransactionType.Expense, "Расходный" },
        { WarehouseTransactionType.Assembly, "Комплектация" },
        { WarehouseTransactionType.Receipt, "Приходный" },
        { WarehouseTransactionType.Adjustment, "Корректировка" },
        { WarehouseTransactionType.Sale, "Реализация" },
        { WarehouseTransactionType.RequirementNote, "Требование-накладная" },
        { WarehouseTransactionType.Advance, "Авансовый" },
        { WarehouseTransactionType.Transfer, "Перемещение" },
        { WarehouseTransactionType.ProductionOutput, "Выпуск" },
        { WarehouseTransactionType.Arrival, "Поступление" },
        { WarehouseTransactionType.Capitalization, "Оприходование" },
        { WarehouseTransactionType.WriteOff, "Списание" },
        { WarehouseTransactionType.Rework, "Переделка" }
    };

    private static readonly Dictionary<string, WarehouseTransactionType> EnglishNames = new()
    {
        { "Return", WarehouseTransactionType.Return },
        { "Expense", WarehouseTransactionType.Expense },
        { "Assembly", WarehouseTransactionType.Assembly },
        { "Receipt", WarehouseTransactionType.Receipt },
        { "Adjustment", WarehouseTransactionType.Adjustment },
        { "Sale", WarehouseTransactionType.Sale },
        { "RequirementNote", WarehouseTransactionType.RequirementNote },
        { "Advance", WarehouseTransactionType.Advance },
        { "Transfer", WarehouseTransactionType.Transfer },
        { "ProductionOutput", WarehouseTransactionType.ProductionOutput },
        { "Arrival", WarehouseTransactionType.Arrival },
        { "Capitalization", WarehouseTransactionType.Capitalization },
        { "WriteOff", WarehouseTransactionType.WriteOff },
        { "Rework", WarehouseTransactionType.Rework }
    };

    public static string ToRussian(this WarehouseTransactionType type)
    {
        if (RussianNames.TryGetValue(type, out var russianName))
            return russianName;

        throw new ArgumentException($"Неизвестный тип операции: {type}");
    }

    public static WarehouseTransactionType FromRussian(string russianName)
    {
        if (string.IsNullOrWhiteSpace(russianName))
            throw new ArgumentException("Русское название не может быть пустым");

        var result = RussianNames.FirstOrDefault(x =>
            string.Equals(x.Value, russianName, StringComparison.OrdinalIgnoreCase));

        if (result.Key == WarehouseTransactionType.Return && !result.Value.Equals(russianName, StringComparison.OrdinalIgnoreCase))
        {
            var found = RussianNames.FirstOrDefault(x =>
                string.Equals(x.Value, russianName, StringComparison.OrdinalIgnoreCase));

            if (!found.Equals(default(KeyValuePair<WarehouseTransactionType, string>)))
                return found.Key;
        }

        throw new ArgumentException($"Неизвестное русское название типа операции: '{russianName}'");
    }

    public static IEnumerable<KeyValuePair<WarehouseTransactionType, string>> GetAllTypes()
    {
        return RussianNames.ToList();
    }

    public static bool IsValidRussianName(string russianName)
    {
        if (string.IsNullOrWhiteSpace(russianName))
            return false;

        return RussianNames.Values.Any(x =>
            string.Equals(x, russianName, StringComparison.OrdinalIgnoreCase));
    }

    public static bool TryFromRussian(string russianName, out WarehouseTransactionType result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(russianName))
            return false;

        var found = RussianNames.FirstOrDefault(x =>
            string.Equals(x.Value, russianName, StringComparison.OrdinalIgnoreCase));

        if (!found.Equals(default(KeyValuePair<WarehouseTransactionType, string>)))
        {
            result = found.Key;
            return true;
        }

        return false;
    }
}
