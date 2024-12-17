namespace Banky.Modules.Clients.Infrastructure;

internal interface IClientsRepository
{
    Task<IReadOnlyCollection<Client>> GetAll(CancellationToken cancellationToken);

    Task<Client?> GetById(Guid clientId, CancellationToken cancellationToken);

    Task Update(Client client, CancellationToken cancellationToken);
}
