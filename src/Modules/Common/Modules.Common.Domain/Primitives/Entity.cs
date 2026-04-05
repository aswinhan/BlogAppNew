// src/Modules/Common/Modules.Common.Domain/Primitives/Entity.cs
namespace Modules.Common.Domain.Primitives;

using Modules.Common.Domain.Events;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid Id { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? ModifiedAtUtc { get; protected set; }

    protected Entity(Guid id)
    {
        Id = id;
        CreatedAtUtc = DateTime.UtcNow;
    }

    protected Entity() { } // For EF Core

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}