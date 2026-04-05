// src/Modules/CRM/Modules.CRM.Domain/Subscribers/Subscriber.cs
namespace Modules.CRM.Domain.Subscribers;

using Modules.Common.Domain.Primitives;

public sealed class Subscriber : Entity
{
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public bool IsActive { get; private set; }

    private Subscriber()
    {
        Email = default!;
        FirstName = default!;
    }

    public static Subscriber Create(string email, string firstName)
    {
        return new Subscriber
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            IsActive = true
        };
    }
}