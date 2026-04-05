// src/Modules/Identity/Modules.Identity.Domain/Users/User.cs
namespace Modules.Identity.Domain.Users;

using Modules.Common.Domain.Primitives;
using Modules.Identity.Domain.Users.Events;

public sealed class User : Entity
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    // Satisfy strict nullability for EF Core reflection
    private User()
    {
        Email = default!;
        PasswordHash = default!;
        FirstName = default!;
        LastName = default!;
    }

    public static User Create(string email, string passwordHash, string firstName, string lastName)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName
        };

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id, user.Email, user.FirstName));

        return user;
    }
}