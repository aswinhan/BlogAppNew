using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modules.Blog.Domain.Categories;
using Modules.Blog.Features;
using Modules.Common.Domain.Results;

namespace Modules.Blog.Features.Categories.Create;

internal sealed class CreateCategoryHandler(IBlogDbContext dbContext) : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(request.Name);

        dbContext.Categories.Add(category);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(category.Id);
    }
}
