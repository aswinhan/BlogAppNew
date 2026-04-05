namespace Modules.Blog.Features.Articles.Search;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Blog.Features.Articles.GetFeed;
using Modules.Common.Domain.Results;

internal sealed class SearchArticlesHandler(IBlogDbContext dbContext) : IRequestHandler<SearchArticlesQuery, Result<PagedResponse<ArticleFeedResponse>>>
{
    public async Task<Result<PagedResponse<ArticleFeedResponse>>> Handle(SearchArticlesQuery request, CancellationToken cancellationToken)
    {
        // 1. Normalize the search term once before the query
        string normalizedSearchTerm = request.SearchTerm.ToLower();

        // 2. Use standard C# methods that EF Core will translate to SQL
        var baseQuery = dbContext.Articles.AsNoTracking()
            .Where(a => 
                a.Title.ToLower().Contains(normalizedSearchTerm) || 
                a.Content.ToLower().Contains(normalizedSearchTerm) || 
                a.Tags.Any(t => t.Name.ToLower().Contains(normalizedSearchTerm)));

        // 3. The rest of the query remains exactly the same
        var query = baseQuery
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