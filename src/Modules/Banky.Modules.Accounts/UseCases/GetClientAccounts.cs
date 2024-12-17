namespace Banky.Modules.Accounts.UseCases;

public sealed class GetClientAccounts
{
    public sealed record Response(IReadOnlyCollection<AccountDto> Accounts);

    internal static async Task<Ok<Response>> Handler(
        Guid clientId,
        IAccountsRepository repository,
        CancellationToken ct
    )
    {
        var accounts = await repository.GetByClient(clientId, ct);

        var accountDtos = accounts
            .Select(AccountDto.FromDomain)
            .OrderBy(a => a.IsClosed)
            .ThenBy(a => a.CreatedOnUtc)
            .ToArray();

        return TypedResults.Ok(new Response(accountDtos));
    }
}
