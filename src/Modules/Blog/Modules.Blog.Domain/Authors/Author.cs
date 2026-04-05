namespace Modules.Blog.Domain.Authors;

using Modules.Common.Domain.Primitives;

public sealed class Author : Entity
{
    public string Email { get; private set; }
    public string Name { get; private set; }

    private Author()
    {
        Email = default!;
        Name = default!;
    }

    public static Author Create(Guid id, string email, string name)
    {
        return new Author
        {
            Id = id,
            Email = email,
            Name = name
        };
    }
}