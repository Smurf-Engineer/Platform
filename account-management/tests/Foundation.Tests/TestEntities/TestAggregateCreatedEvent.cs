using PlatformPlatform.Foundation.DddCqrsFramework.DomainEvents;

namespace PlatformPlatform.Foundation.Tests.TestEntities;

public record TestAggregateCreatedEvent(long TestAggregateId, string Name) : IDomainEvent;