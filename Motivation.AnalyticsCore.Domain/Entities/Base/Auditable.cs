using Motivation.AnalyticsCore.Domain.Interfaces;

namespace Motivation.AnalyticsCore.Domain.Entities.Base;

public class Auditable : IAuditable
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
