namespace Motivation.AnalyticsCore.Domain.Enum;

public enum WarehouseTransactionType
{
    /// <summary>Возврат</summary>
    Return = 1,

    /// <summary>Расходный</summary>
    Expense = 2,

    /// <summary>Комплектация</summary>
    Assembly = 3,

    /// <summary>Приходный</summary>
    Receipt = 4,

    /// <summary>Корректировка</summary>
    Adjustment = 5,

    /// <summary>Реализация</summary>
    Sale = 6,

    /// <summary>Требование-накладная</summary>
    RequirementNote = 7,

    /// <summary>Авансовый</summary>
    Advance = 8,

    /// <summary>Перемещение</summary>
    Transfer = 9,

    /// <summary>Выпуск</summary>
    ProductionOutput = 10,

    /// <summary>Поступление</summary>
    Arrival = 11,

    /// <summary>Оприходование</summary>
    Capitalization = 12,

    /// <summary>Списание</summary>
    WriteOff = 13,

    /// <summary>Переделка</summary>
    Rework = 14
}
