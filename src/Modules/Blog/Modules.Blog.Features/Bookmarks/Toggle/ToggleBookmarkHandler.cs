namespace Modules.Blog.Features.Bookmarks.Toggle;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Errors;
using Modules.Common.Domain.Results;
using Modules.Blog.Domain.Bookmarks;

internal sealed class ToggleBookmarkHandler(IBlogDbContext dbContext) 
    : IRequestHandler<ToggleBookmarkCommand, Result>
{
    public async Task<Result> Handle(ToggleBookmarkCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if the bookmark already exists
        var existingBookmark = await dbContext.Bookmarks
            .FirstOrDefaultAsync(b => b.UserId == request.UserId && b.ArticleId == request.ArticleId, cancellationToken);

        if (existingBookmark is not null)
        {
            // If it exists, the user is "Un-saving" the article
            dbContext.Bookmarks.Remove(existingBookmark);
        }
        else
        {
            // 2. If it doesn't exist, verify the article actually exists before saving
            bool articleExists = await dbContext.Articles
                .AnyAsync(a => a.Id == request.ArticleId, cancellationToken);

            if (!articleExists)
            {
                return Result.Failure(new Error("Article.NotFound", "The specified article does not exist."));
            }

            // If it doesn't exist, the user is "Saving" the article
            var newBookmark = Bookmark.Create(request.UserId, request.ArticleId);
            dbContext.Bookmarks.Add(newBookmark);
        }

        // 3. Commit the transaction to the database
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}