using FluentValidation;

namespace Banky.Modules.Accounts.UseCases;

public sealed class OpenAccount
{
    public sealed record Request(string AccountName, AccountType Type, decimal InitialBalance);

    public sealed record Response(AccountDto Account);

    internal sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.AccountName).NotEmpty().MinimumLength(5).MaximumLength(50);
            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.InitialBalance).GreaterThan(0).LessThanOrEqualTo(100_000);
        }
    }

    internal static async Task<Results<Ok<Response>, BadRequest<IEnumerable<string>>>> Handler(
        Guid clientId,
        Request request,
        IAccountsRepository repository,
        TimeProvider timeProvider,
        CancellationToken ct
    )
    {
        var validation = new Validator().Validate(request);

        if (!validation.IsValid)
            return TypedResults.BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = Account
            .Create(clientId, request.Type, request.AccountName, timeProvider)
            .Bind(acct => acct.Deposit(request.InitialBalance));

        if (result.IsFailed)
            return TypedResults.BadRequest(result.Errors.Select(e => e.Message));

        var account = result.Value;

        await repository.Insert(account, ct);

        var accountDto = AccountDto.FromDomain(account);
        return TypedResults.Ok(new Response(accountDto));
    }
}
