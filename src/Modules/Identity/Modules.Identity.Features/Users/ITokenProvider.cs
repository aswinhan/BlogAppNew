// src/Modules/Identity/Modules.Identity.Features/Users/ITokenProvider.cs
namespace Modules.Identity.Features.Users;

using Modules.Identity.Domain.Users;

public interface ITokenProvider
{
    string Create(User user);
}