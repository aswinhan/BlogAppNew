// src/Modules/Common/Modules.Common.Domain/Errors/Error.cs
namespace Modules.Common.Domain.Errors;

public record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");
    public static readonly Error InvalidCredentials = new("Auth.InvalidCredentials", "Invalid email or password.");
    public static readonly Error EmailNotUnique = new("Auth.EmailNotUnique", "The provided email is already in use.");
}