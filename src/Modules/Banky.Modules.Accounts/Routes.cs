using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Banky.Modules.Accounts;

public static class Routes
{
    public static RouteGroupBuilder MapAccountsRoutes(this RouteGroupBuilder routes)
    {
        var accountRoutes = routes.MapGroup("/accounts").WithTags("Accounts");

        accountRoutes
            .MapGet("/{clientId}", GetClientAccounts.Handler)
            .CacheOutput(p => p.Expire(TimeSpan.FromSeconds(5)))
            .WithDescription("Get all accounts attached to a client");

        accountRoutes
            .MapPost("/{clientId}", OpenAccount.Handler)
            .WithDescription("Open a new account for a client");

        accountRoutes
            .MapPost("/{clientId}/{accountId}/transfer", TransferBetweenAccounts.Handler)
            .WithDescription("Transfer a given amount between accounts");

        accountRoutes
            .MapPost("/{clientId}/{accountId}/close", CloseAccount.Handler)
            .WithDescription("Close an account");

        accountRoutes
            .MapPut("/{clientId}/{accountId}", UpdateAccount.Handler)
            .WithDescription("Update an account");

        return accountRoutes;
    }
}
