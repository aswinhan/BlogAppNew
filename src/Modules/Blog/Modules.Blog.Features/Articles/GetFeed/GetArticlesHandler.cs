using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Results;

namespace Modules.Blog.Features.Articles.GetFeed;

internal sealed class GetArticlesHandler(IBlogDbContext dbContext) : IRequestHandler<GetArticlesQuery, Result<PagedResponse<ArticleFeedResponse>>>
{
    public async Task<Result<PagedResponse<ArticleFeedResponse>>> Handle(GetArticlesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Articles.AsNoTracking()
            .Join(
                dbContext.Categories,
                a => a.CategoryId,
                c => c.Id,
                (a, c) => new { Article = a, Category = c }
            )
            .Join(
                dbContext.Authors,
                ac => ac.Article.AuthorId,
                au => au.Id,
                (ac, au) => new { Article = ac.Article, Category = ac.Category, Author = au }
            );

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.Article.CreatedAtUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ArticleFeedResponse(
                x.Article.Id,
                x.Article.Title,
                x.Article.Slug,
                x.Category.Name,
                x.Author.Name,
                x.Article.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return Result.Success(new PagedResponse<ArticleFeedResponse>(items, totalCount, request.Page, request.PageSize));
    }
}
