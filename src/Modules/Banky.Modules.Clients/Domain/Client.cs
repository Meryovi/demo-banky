using FluentResults;

namespace Banky.Modules.Clients.Domain;

internal sealed class Client
{
    public Guid Id { get; private set; }
    public ClientType Type { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? LastName { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public DateTime? BirthDate { get; private set; }
    public DateTimeOffset CreatedOnUtc { get; private set; }
    public DateTimeOffset? UpdatedOnUtc { get; private set; }

    internal const int PersonLegalAge = 18;

    private Client() { }

    public static Result<Client> Create(
        ClientType type,
        string name,
        string? lastName,
        string email,
        DateTime? birthDate,
        TimeProvider time,
        Guid? id = null
    )
    {
        var validation = ValidateClientData(type, name, email, lastName, birthDate, time);

        if (validation.IsFailed)
            return validation;

        var client = new Client
        {
            Id = id ?? Guid.CreateVersion7(),
            Type = type,
            Name = name,
            Email = email,
            LastName = lastName,
            BirthDate = birthDate,
            CreatedOnUtc = time.GetUtcNow()
        };

        return client;
    }

    public Result<Client> Update(
        string name,
        string? lastName,
        string email,
        DateTime? birthDate,
        TimeProvider time
    )
    {
        var validation = ValidateClientData(Type, name, email, lastName, birthDate, time);

        if (validation.IsFailed)
            return validation;

        Name = name;
        Email = email;
        LastName = lastName;
        BirthDate = birthDate;
        UpdatedOnUtc = time.GetUtcNow();

        return this;
    }

    private static Result ValidateClientData(
        ClientType type,
        string name,
        string email,
        string? lastName,
        DateTime? birthDate,
        TimeProvider time
    )
    {
        var result = Result.Ok();

        if (string.IsNullOrWhiteSpace(name))
            result.WithError(Errors.NameIsRequired());

        if (string.IsNullOrWhiteSpace(email))
            result.WithError(Errors.EmailIsRequired());

        if (type == ClientType.Person)
        {
            if (birthDate is null)
                result.WithError(Errors.BirthDateIsRequiredForPersonClients());

            if (birthDate > time.GetUtcNow())
                result.WithError(Errors.BirthDateCannotBeInTheFuture());

            if (birthDate is not null && time.GetUtcNow().Year - birthDate.Value.Year < PersonLegalAge)
                result.WithError(Errors.MustBeOfLegalAge(PersonLegalAge));

            if (string.IsNullOrEmpty(lastName))
                result.WithError(Errors.LastNameIsRequired());
        }

        return result;
    }

    internal static Client Create(
        Guid id,
        ClientType type,
        string name,
        string? lastName,
        string email,
        DateTime? birthDate,
        DateTimeOffset createdOnUtc
    )
    {
        return new Client
        {
            Id = id,
            Type = type,
            Name = name,
            LastName = lastName,
            Email = email,
            BirthDate = birthDate,
            CreatedOnUtc = createdOnUtc
        };
    }
}
