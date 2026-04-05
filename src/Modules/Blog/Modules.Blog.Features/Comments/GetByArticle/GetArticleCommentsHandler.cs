namespace Modules.Blog.Features.Comments.GetByArticle;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Results;

public sealed class GetArticleCommentsHandler(IBlogDbContext dbContext) : IRequestHandler<GetArticleCommentsQuery, Result<List<CommentResponse>>>
{
    public async Task<Result<List<CommentResponse>>> Handle(GetArticleCommentsQuery request, CancellationToken cancellationToken)
    {
        var commentsData = await (from c in dbContext.Comments.AsNoTracking()
                                  join a in dbContext.Authors.AsNoTracking() on c.UserId equals a.Id into authorJoin
                                  from a in authorJoin.DefaultIfEmpty()
                                  where c.ArticleId == request.ArticleId
                                  select new
                                  {
                                      c.Id,
                                      c.Content,
                                      AuthorName = a != null ? a.Name : "Unknown",
                                      AuthorAvatarUrl = a != null ? a.AvatarUrl : null,
                                      c.CreatedAtUtc,
                                      c.ParentCommentId
                                  })
                                  .OrderBy(c => c.CreatedAtUtc)
                                  .ToListAsync(cancellationToken);

        var allComments = commentsData.Select(c => new CommentResponse(
            c.Id,
            c.Content,
            c.AuthorName,
            c.AuthorAvatarUrl,
            c.CreatedAtUtc,
            c.ParentCommentId,
            new List<CommentResponse>()
        )).ToList();

        var lookup = allComments.ToLookup(c => c.ParentCommentId);

        foreach (var comment in allComments)
        {
            comment.Replies.AddRange(lookup[comment.Id]);
        }

        var rootComments = lookup[null].ToList();

        return Result.Success(rootComments);
    }
}
