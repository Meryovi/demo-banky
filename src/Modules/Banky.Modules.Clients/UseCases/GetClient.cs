namespace Banky.Modules.Clients.UseCases;

public sealed class GetClient
{
    public sealed record Response(ClientDto Client);

    internal static async Task<Results<Ok<Response>, NotFound>> Handler(
        Guid clientId,
        IClientsRepository repository,
        CancellationToken ct
    )
    {
        var client = await repository.GetById(clientId, ct);

        if (client is null)
            return TypedResults.NotFound();

        var clientDto = ClientDto.FromDomain(client);
        return TypedResults.Ok(new Response(clientDto));
    }
}
