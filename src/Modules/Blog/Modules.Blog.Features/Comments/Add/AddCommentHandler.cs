namespace Modules.Blog.Features.Comments.Add;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Blog.Domain.Comments;
using Modules.Common.Domain.Errors;
using Modules.Common.Domain.Results;

public sealed class AddCommentHandler(IBlogDbContext dbContext) : IRequestHandler<AddCommentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var articleExists = await dbContext.Articles.AnyAsync(a => a.Id == request.ArticleId, cancellationToken);
        if (!articleExists)
        {
            return Result.Failure<Guid>(new Error("Article.NotFound", "The specified article does not exist."));
        }

        if (request.ParentCommentId.HasValue)
        {
            var parentExists = await dbContext.Comments.AnyAsync(c => c.Id == request.ParentCommentId.Value, cancellationToken);
            if (!parentExists)
            {
                return Result.Failure<Guid>(new Error("Comment.NotFound", "The specified parent comment does not exist."));
            }
        }

        var comment = Comment.Create(request.ArticleId, request.UserId, request.Content, request.ParentCommentId);

        dbContext.Comments.Add(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(comment.Id);
    }
}
