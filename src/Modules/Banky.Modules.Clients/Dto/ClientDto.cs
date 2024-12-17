namespace Banky.Modules.Clients.Dto;

public sealed record ClientDto(
    Guid Id,
    ClientType Type,
    string Name,
    string? LastName,
    string Email,
    DateTime? BirthDate,
    DateTimeOffset CreatedOnUtc,
    DateTimeOffset? UpdatedOnUtc
)
{
    internal static ClientDto FromDomain(Client client)
    {
        return new ClientDto(
            client.Id,
            client.Type,
            client.Name,
            client.LastName,
            client.Email,
            client.BirthDate,
            client.CreatedOnUtc,
            client.UpdatedOnUtc
        );
    }
}
