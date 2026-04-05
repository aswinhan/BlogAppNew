namespace Modules.Blog.Features.Bookmarks.GetSaved;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Results;
using Modules.Blog.Features.Articles.GetFeed; // Imports the DTOs

internal sealed class GetSavedArticlesHandler(IBlogDbContext dbContext) 
    : IRequestHandler<GetSavedArticlesQuery, Result<PagedResponse<ArticleFeedResponse>>>
{
    public async Task<Result<PagedResponse<ArticleFeedResponse>>> Handle(GetSavedArticlesQuery request, CancellationToken cancellationToken)
    {
        // Using LINQ Query Syntax because it is much cleaner for multiple joins than Method Syntax
        var query = from b in dbContext.Bookmarks.AsNoTracking()
                    where b.UserId == request.UserId
                    join a in dbContext.Articles on b.ArticleId equals a.Id
                    join c in dbContext.Categories on a.CategoryId equals c.Id
                    join auth in dbContext.Authors on a.AuthorId equals auth.Id
                    orderby b.CreatedAtUtc descending
                    select new ArticleFeedResponse(
                        a.Id,
                        a.Title,
                        a.Slug,
                        c.Name,
                        auth.Name,
                        a.CreatedAtUtc
                    );

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var response = new PagedResponse<ArticleFeedResponse>(items, totalCount, request.Page, request.PageSize);

        return Result.Success(response);
    }
}