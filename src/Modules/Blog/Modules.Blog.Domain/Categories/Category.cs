using System;
using Modules.Common.Domain.Primitives;

namespace Modules.Blog.Domain.Categories;

public sealed class Category : Entity
{
    public string Name { get; private set; }
    public string Slug { get; private set; }

    private Category()
    {
        Name = default!;
        Slug = default!;
    }

    private Category(Guid id, string name, string slug) : base(id)
    {
        Name = name;
        Slug = slug;
    }

    public static Category Create(string name)
    {
        string slug = name.ToLowerInvariant().Replace(" ", "-");
        return new Category(Guid.NewGuid(), name, slug);
    }
}
