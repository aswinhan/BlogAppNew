// src/Modules/Common/Modules.Common.API/Abstractions/IApiEndpoint.cs
namespace Modules.Common.API.Abstractions;

using Microsoft.AspNetCore.Routing;

public interface IApiEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}