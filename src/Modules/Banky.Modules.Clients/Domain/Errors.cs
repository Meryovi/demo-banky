using FluentResults;

namespace Banky.Modules.Clients.Domain;

internal static class Errors
{
    public static Error NameIsRequired() => new("Name is required");

    public static Error EmailIsRequired() => new("Email is required");

    public static Error BirthDateIsRequiredForPersonClients() => new("Birth date is required for person clients");

    public static Error BirthDateCannotBeInTheFuture() => new("Birth date cannot be in the future");

    public static Error MustBeOfLegalAge(int legalAge) => new($"Clients must be at least {legalAge} years old");

    public static Error LastNameIsRequired() => new("Last name is required for person clients");
}
