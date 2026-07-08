using Mediator;

namespace Motivation.AnalyticsCore.Domain.Interfaces;

//[JsonDerivedType(typeof(PositionCreatedDomainEvent), typeDiscriminator: "position-created")]

public interface IDomainEvent : INotification { }
