// src/Modules/Common/Modules.Common.Domain/Events/IDomainEvent.cs
namespace Modules.Common.Domain.Events;

using MediatR;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredOnUtc { get; }
}