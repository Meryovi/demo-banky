using FluentValidation;

namespace Banky.Modules.Accounts.UseCases;

public sealed class TransferBetweenAccounts
{
    public sealed record Request(Guid ToAccountId, decimal Amount)
    {
        internal Guid FromAccountId { get; set; }
    }

    public sealed record Response(AccountDto Account);

    internal sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.FromAccountId).NotEmpty();
            RuleFor(x => x.ToAccountId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.FromAccountId)
                .NotEqual(x => x.ToAccountId)
                .WithMessage("Cannot transfer to the same account.");
        }
    }

    internal static async Task<Results<Ok<Response>, NotFound, BadRequest<IEnumerable<string>>>> Handler(
        Guid clientId,
        Guid accountId,
        Request request,
        IAccountsRepository repository,
        CancellationToken ct
    )
    {
        request.FromAccountId = accountId;

        var validation = new Validator().Validate(request);

        if (!validation.IsValid)
            return TypedResults.BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var originAccount = await repository.GetById(request.FromAccountId, ct);
        var destinationAccount = await repository.GetById(request.ToAccountId, ct);

        if (originAccount is null || destinationAccount is null || originAccount.ClientId != clientId)
            return TypedResults.NotFound();

        var result = originAccount.TransferTo(destinationAccount, request.Amount);

        if (result.IsFailed)
            return TypedResults.BadRequest(result.Errors.Select(e => e.Message));

        await repository.Update(ct, originAccount, destinationAccount);

        var accountDto = AccountDto.FromDomain(originAccount);
        return TypedResults.Ok(new Response(accountDto));
    }
}
