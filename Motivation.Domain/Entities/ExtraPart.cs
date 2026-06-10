using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public sealed class ExtraPart : SimpleEntity
{
    private ExtraPart(Guid id, TitleValue title, CodeValue code, ExtraPartValue extraPartValue, DateTime validFrom, DateTime validTo) : base(title, code, id)
    {
        ExtraPartValue = extraPartValue;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public ExtraPartValue ExtraPartValue { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidTo { get; private set; }

    public EmployeePosition EmployeePosition { get; private set; } = null!;

    public static Operation<ExtraPart, string> Create(TitleValue title, CodeValue code, ExtraPartValue extraPart, DateTime validFrom, DateTime validTo = default)
    {
        if (title is null)
            return Operation.Error("Title is null!");
        if (code is null)
            return Operation.Error("Code is null!");
        if (extraPart is null)
            return Operation.Error("Extra part is null!");

        return new ExtraPart(Guid.CreateVersion7(), title, code, extraPart, validFrom, validTo);
    }

    public Operation<bool, string> SetValidTo(DateTime validTo)
    {
        ValidTo = validTo;
        return true;
    }
}