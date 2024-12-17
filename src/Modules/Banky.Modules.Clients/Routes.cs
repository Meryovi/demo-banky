using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Banky.Modules.Clients;

public static class Routes
{
    public static RouteGroupBuilder MapClientsRoutes(this RouteGroupBuilder routes)
    {
        var clientRoutes = routes.MapGroup("/clients").WithTags("Clients");

        clientRoutes
            .MapGet("/", GetAllClients.Handler)
            .CacheOutput(p => p.Expire(TimeSpan.FromSeconds(10)))
            .WithDescription("Get all clients");

        clientRoutes
            .MapGet("/{clientId}", GetClient.Handler)
            .CacheOutput(p => p.Expire(TimeSpan.FromSeconds(5)))
            .WithDescription("Get a client by its ID");

        clientRoutes
            .MapPut("/{clientId}", UpdateClient.Handler)
            .WithDescription("Update a client's profile by its ID");

        return clientRoutes;
    }
}
