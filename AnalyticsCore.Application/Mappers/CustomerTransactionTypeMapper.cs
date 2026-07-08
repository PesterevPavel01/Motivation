namespace AnalyticsCore.Application.Mappers;

public static class CustomerTransactionTypeMapper
{
    private static readonly Dictionary<CustomerTransactionType, string> RussianNames = new()
    {
        { CustomerTransactionType.Sale, "Реализация" },
        { CustomerTransactionType.Adjustment, "Корректировка" },
        { CustomerTransactionType.Return, "Возврат" }
    };

    public static string ToRussian(this CustomerTransactionType type)
    {
        if (RussianNames.TryGetValue(type, out var russianName))
            return russianName;

        throw new ArgumentException($"Неизвестный тип операции: {type}");
    }

    public static CustomerTransactionType FromRussian(string russianName)
    {
        if (string.IsNullOrWhiteSpace(russianName))
            throw new ArgumentException("Русское название не может быть пустым");

        var found = RussianNames.FirstOrDefault(x =>
            string.Equals(x.Value, russianName, StringComparison.OrdinalIgnoreCase));

        if (!found.Equals(default(KeyValuePair<CustomerTransactionType, string>)))
            return found.Key;

        throw new ArgumentException($"Неизвестное русское название типа операции: '{russianName}'");
    }

    public static IEnumerable<KeyValuePair<CustomerTransactionType, string>> GetAllTypes()
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

    public static bool TryFromRussian(string russianName, out CustomerTransactionType result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(russianName))
            return false;

        var found = RussianNames.FirstOrDefault(x =>
            string.Equals(x.Value, russianName, StringComparison.OrdinalIgnoreCase));

        if (!found.Equals(default(KeyValuePair<CustomerTransactionType, string>)))
        {
            result = found.Key;
            return true;
        }

        return false;
    }
}
