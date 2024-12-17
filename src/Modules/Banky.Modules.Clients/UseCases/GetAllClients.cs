namespace Banky.Modules.Clients.UseCases;

public sealed class GetAllClients
{
    public sealed record Response(IReadOnlyCollection<ClientDto> Clients);

    internal static async Task<Results<Ok<Response>, NotFound>> Handler(
        IClientsRepository repository,
        CancellationToken ct
    )
    {
        var clients = await repository.GetAll(ct);
        var clientsDto = clients.Select(ClientDto.FromDomain).ToArray();

        return TypedResults.Ok(new Response(clientsDto));
    }
}
