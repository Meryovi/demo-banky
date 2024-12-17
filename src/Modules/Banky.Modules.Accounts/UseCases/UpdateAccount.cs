namespace Banky.Modules.Accounts.UseCases;

public sealed class UpdateAccount
{
    public sealed record Response(AccountDto Account);

    public sealed record Request(string Name);

    internal static async Task<Results<Ok<Response>, NotFound, BadRequest<IEnumerable<string>>>> Handler(
        Guid clientId,
        Guid accountId,
        Request request,
        IAccountsRepository repository,
        CancellationToken ct
    )
    {
        var account = await repository.GetById(accountId, ct);

        if (account is null || account.ClientId != clientId)
            return TypedResults.NotFound();

        var result = account.Update(request.Name);

        if (result.IsFailed)
            return TypedResults.BadRequest(result.Errors.Select(e => e.Message));

        await repository.Update(ct, account);

        var accountDto = AccountDto.FromDomain(account);
        return TypedResults.Ok(new Response(accountDto));
    }
}
