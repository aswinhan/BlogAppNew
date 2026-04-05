// src/Modules/Identity/Modules.Identity.Features/Users/Register/RegisterUserHandler.cs
namespace Modules.Identity.Features.Users.Register;

using MediatR;
using Modules.Common.Domain.Results;
using Modules.Common.Domain.Errors;
using Modules.Identity.Domain.Users;
using BCrypt.Net;

public sealed class RegisterUserHandler(IIdentityDbContext dbContext) : IRequestHandler<RegisterUserCommand, Result>
{
    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Check uniqueness (Exception-free control flow)
        if (dbContext.Users.Any(u => u.Email == request.Email))
        {
            return Result.Failure(Error.EmailNotUnique);
        }

        // 2. Hash Password securely
        string hash = BCrypt.HashPassword(request.Password);

        // 3. Create & Save
        var user = User.Create(request.Email, hash, request.FirstName, request.LastName);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}