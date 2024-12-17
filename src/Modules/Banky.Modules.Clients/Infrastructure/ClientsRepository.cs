namespace Banky.Modules.Clients.Infrastructure;

internal sealed class ClientsRepository(BankyClientsDbContext dbContext) : IClientsRepository
{
    public async Task<IReadOnlyCollection<Client>> GetAll(CancellationToken cancellationToken)
    {
        return await dbContext.Clients.ToArrayAsync(cancellationToken);
    }

    public Task<Client?> GetById(Guid clientId, CancellationToken cancellationToken)
    {
        return dbContext.Clients.FindAsync([clientId], cancellationToken).AsTask();
    }

    public Task Update(Client client, CancellationToken cancellationToken)
    {
        dbContext.Clients.Update(client);

        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
