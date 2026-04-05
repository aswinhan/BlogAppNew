// src/Modules/Identity/Modules.Identity.Features/Users/Login/LoginUserHandler.cs
namespace Modules.Identity.Features.Users.Login;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Results;
using Modules.Common.Domain.Errors;
using Modules.Identity.Features.Users; 

public sealed class LoginUserHandler(
    IIdentityDbContext dbContext,
    ITokenProvider tokenProvider) : IRequestHandler<LoginUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<string>(Error.InvalidCredentials);
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            return Result.Failure<string>(Error.InvalidCredentials);
        }

        // We use the interface here. Zero coupling to Infrastructure!
        string token = tokenProvider.Create(user);

        return Result.Success(token);
    }
}