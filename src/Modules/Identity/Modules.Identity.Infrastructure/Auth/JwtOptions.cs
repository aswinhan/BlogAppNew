// src/Modules/Identity/Modules.Identity.Infrastructure/Auth/JwtOptions.cs
namespace Modules.Identity.Infrastructure.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
}