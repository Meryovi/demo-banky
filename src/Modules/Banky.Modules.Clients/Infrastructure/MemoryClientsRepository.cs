namespace Banky.Modules.Clients.Infrastructure;

internal sealed class MemoryClientsRepository : IClientsRepository
{
    private readonly Client[] _clients =
    [
        Client
            .Create(
                ClientType.Person,
                "John",
                "Doe",
                "john@doe.com",
                new(1988, 5, 8),
                TimeProvider.System,
                new("79c03439-4b69-4b42-a6a5-ea17972d4ad4")
            )
            .Value,
        Client
            .Create(ClientType.Person, "Nancy", "Smith", "nsmith@gmail.com", new(2001, 8, 1), TimeProvider.System)
            .Value,
        Client.Create(ClientType.Company, "Acme Co.", null, "john@doe.com", null, TimeProvider.System).Value,
    ];

    public Task<IReadOnlyCollection<Client>> GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<Client>>(_clients);
    }

    public Task<Client?> GetById(Guid clientId, CancellationToken cancellationToken)
    {
        var client = _clients.FirstOrDefault(c => c.Id == clientId);
        return Task.FromResult(client);
    }

    public Task Update(Client client, CancellationToken cancellationToken)
    {
        // Would have been updated in memory.
        return Task.CompletedTask;
    }
}
