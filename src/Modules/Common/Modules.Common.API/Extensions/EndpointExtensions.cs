// src/Modules/Common/Modules.Common.API/Extensions/EndpointExtensions.cs
namespace Modules.Common.API.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var endpointTypes = assembly.GetTypes()
            .Where(t => typeof(IApiEndpoint).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in endpointTypes)
        {
            services.AddTransient(typeof(IApiEndpoint), type);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.ServiceProvider.GetServices<IApiEndpoint>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}