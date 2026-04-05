using System;
using System.Collections.Generic;
using Modules.Common.Domain.Primitives;

namespace Modules.Blog.Domain.Tags;

public sealed class Tag : Entity
{
    public string Name { get; private set; }
    public string Slug { get; private set; }

    private readonly List<Modules.Blog.Domain.Articles.Article> _articles = [];
    public IReadOnlyCollection<Modules.Blog.Domain.Articles.Article> Articles => _articles.AsReadOnly();

    private Tag()
    {
        Name = default!;
        Slug = default!;
    }

    public static Tag Create(string name)
    {
        return new Tag
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = name.ToLowerInvariant().Replace(" ", "-"),
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}
