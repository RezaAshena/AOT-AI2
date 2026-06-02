using MediatR;

namespace AOT.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTime OccurredAt { get; }
}

public abstract class BaseDomainEvent : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
