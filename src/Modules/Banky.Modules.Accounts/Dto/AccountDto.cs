namespace Banky.Modules.Accounts.Dto;

public sealed record AccountDto(
    Guid Id,
    Guid ClientId,
    AccountType Type,
    string Name,
    decimal Balance,
    bool IsClosed,
    DateTimeOffset CreatedOnUtc,
    DateTimeOffset? ClosedOnUtc
)
{
    internal static AccountDto FromDomain(Account account)
    {
        return new AccountDto(
            account.Id,
            account.ClientId,
            account.Type,
            account.Name,
            account.Balance,
            account.IsClosed,
            account.CreatedOnUtc,
            account.ClosedOnUtc
        );
    }
}
