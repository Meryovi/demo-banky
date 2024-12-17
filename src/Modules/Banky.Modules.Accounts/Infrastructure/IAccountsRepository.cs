namespace Banky.Modules.Accounts.Infrastructure;

internal interface IAccountsRepository
{
    Task<IReadOnlyCollection<Account>> GetByClient(Guid clientId, CancellationToken cancellationToken);

    Task<Account?> GetById(Guid accountId, CancellationToken cancellationToken);

    Task Insert(Account account, CancellationToken cancellationToken);

    Task Update(CancellationToken cancellationToken, params IEnumerable<Account> accounts);
}
