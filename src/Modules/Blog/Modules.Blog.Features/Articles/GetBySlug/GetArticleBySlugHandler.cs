using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Modules.Blog.Features;
using Modules.Common.Domain.Errors;
using Modules.Common.Domain.Results;

namespace Modules.Blog.Features.Articles.GetBySlug;

internal sealed class GetArticleBySlugHandler : IRequestHandler<GetArticleBySlugQuery, Result<ArticleResponse>>
{
    private readonly IBlogDbContext _dbContext;
    private readonly IDistributedCache _cache;

    public GetArticleBySlugHandler(IBlogDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<Result<ArticleResponse>> Handle(GetArticleBySlugQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = $"article:{request.Slug}";

        string? cachedArticleStr = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedArticleStr))
        {
            var cachedArticle = JsonSerializer.Deserialize<ArticleResponse>(cachedArticleStr);
            if (cachedArticle is not null)
            {
                return Result.Success(cachedArticle);
            }
        }

        var article = await _dbContext.Articles
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Slug == request.Slug, cancellationToken);

        if (article is null)
        {
            return Result.Failure<ArticleResponse>(new Error("Article.NotFound", "Article not found."));
        }

        var response = new ArticleResponse(
            article.Id,
            article.Title,
            article.Content,
            article.CreatedAtUtc
        );

        string serializedResponse = JsonSerializer.Serialize(response);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
        };

        await _cache.SetStringAsync(cacheKey, serializedResponse, cacheOptions, cancellationToken);

        return Result.Success(response);
    }
}
