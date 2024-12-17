using FluentValidation;

namespace Banky.Modules.Clients.UseCases;

public sealed class UpdateClient
{
    public sealed record Request(string Name, string? LastName, string Email, DateTime? BirthDate);

    public sealed record Response(ClientDto Client);

    internal sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email)
                .NotEmpty()
                .Must(x => System.Net.Mail.MailAddress.TryCreate(x, out _))
                .WithMessage(x => $"The email address '{x.Email}' is invalid.");
        }
    }

    internal static async Task<Results<Ok<Response>, NotFound, BadRequest<IEnumerable<string>>>> Handler(
        Guid clientId,
        Request request,
        IClientsRepository repository,
        TimeProvider timeProvider,
        CancellationToken ct
    )
    {
        var validation = new Validator().Validate(request);

        if (!validation.IsValid)
            return TypedResults.BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var client = await repository.GetById(clientId, ct);

        if (client is null)
            return TypedResults.NotFound();

        var result = client.Update(request.Name, request.LastName, request.Email, request.BirthDate, timeProvider);

        if (result.IsFailed)
            return TypedResults.BadRequest(result.Errors.Select(e => e.Message));

        await repository.Update(client, ct);

        var clientDto = ClientDto.FromDomain(client);
        return TypedResults.Ok(new Response(clientDto));
    }
}
