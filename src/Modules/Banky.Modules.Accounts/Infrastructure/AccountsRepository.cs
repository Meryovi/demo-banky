namespace Banky.Modules.Accounts.Infrastructure;

internal sealed class AccountsRepository(BankyAccountsDbContext dbContext) : IAccountsRepository
{
    public async Task<IReadOnlyCollection<Account>> GetByClient(Guid clientId, CancellationToken cancellationToken)
    {
        return await dbContext.Accounts.Where(a => a.ClientId == clientId).ToArrayAsync(cancellationToken);
    }

    public Task<Account?> GetById(Guid accountId, CancellationToken cancellationToken)
    {
        return dbContext.Accounts.FindAsync([accountId], cancellationToken).AsTask();
    }

    public Task Insert(Account account, CancellationToken cancellationToken)
    {
        dbContext.Accounts.Add(account);

        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task Update(CancellationToken cancellationToken, params IEnumerable<Account> accounts)
    {
        foreach (var account in accounts)
        {
            dbContext.Accounts.Update(account);
        }

        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
