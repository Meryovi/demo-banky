namespace Banky.Modules.Accounts.UseCases;

public sealed class CloseAccount
{
    public sealed record Response(AccountDto Account);

    internal static async Task<Results<Ok<Response>, NotFound, BadRequest<IEnumerable<string>>>> Handler(
        Guid clientId,
        Guid accountId,
        IAccountsRepository repository,
        TimeProvider timeProvider,
        CancellationToken ct
    )
    {
        var account = await repository.GetById(accountId, ct);

        if (account is null || account.ClientId != clientId)
            return TypedResults.NotFound();

        var result = account.Close(timeProvider);

        if (result.IsFailed)
            return TypedResults.BadRequest(result.Errors.Select(e => e.Message));

        await repository.Update(ct, account);

        var accountDto = AccountDto.FromDomain(account);
        return TypedResults.Ok(new Response(accountDto));
    }
}
