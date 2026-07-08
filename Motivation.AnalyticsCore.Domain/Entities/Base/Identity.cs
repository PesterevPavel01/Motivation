using Motivation.AnalyticsCore.Domain.Interfaces;

namespace Motivation.AnalyticsCore.Domain.Entities.Base;

/// <summary>
/// Identifier
/// </summary>
public abstract class Identity : IHaveId
{
    /// <summary>
    /// Identifier
    /// </summary>
    public Guid Id { get; set; }
}