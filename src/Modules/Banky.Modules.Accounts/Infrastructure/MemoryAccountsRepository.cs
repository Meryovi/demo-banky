using System.Collections.Concurrent;

namespace Banky.Modules.Accounts.Infrastructure;

internal sealed class MemoryAccountsRepository : IAccountsRepository
{
    private readonly ConcurrentBag<Account> _accounts =
    [
        Account
            .Create(
                new("79c03439-4b69-4b42-a6a5-ea17972d4ad4"),
                AccountType.Savings,
                "My account",
                TimeProvider.System
            )
            .Value.Deposit(4562.36M)
            .Value
    ];

    public Task<IReadOnlyCollection<Account>> GetByClient(Guid clientId, CancellationToken cancellationToken)
    {
        var accounts = _accounts.Where(a => a.ClientId == clientId).ToArray();
        return Task.FromResult<IReadOnlyCollection<Account>>(accounts);
    }

    public Task<Account?> GetById(Guid accountId, CancellationToken cancellationToken)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == accountId);
        return Task.FromResult(account);
    }

    public Task Insert(Account account, CancellationToken cancellationToken)
    {
        _accounts.Add(account);

        return Task.CompletedTask;
    }

    public Task Update(CancellationToken cancellationToken, params IEnumerable<Account> accounts)
    {
        // Item would have been updated in memory.
        return Task.CompletedTask;
    }
}
