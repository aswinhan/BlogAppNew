// src/Modules/Blog/Modules.Blog.Features/Articles/Create/CreateArticleHandler.cs
namespace Modules.Blog.Features.Articles.Create;

using MediatR;
using Modules.Common.Domain.Results;
using Modules.Common.Domain.Errors;
using Modules.Blog.Domain.Articles;

public sealed class CreateArticleHandler(IBlogDbContext dbContext) : IRequestHandler<CreateArticleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        var article = Article.Create(request.Title, request.Content, request.AuthorId, request.CategoryId);

        // For testing the Domain Event locally, let's auto-publish it upon creation
        article.Publish();

        dbContext.Articles.Add(article);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(article.Id);
    }
}