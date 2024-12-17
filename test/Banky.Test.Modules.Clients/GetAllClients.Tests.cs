using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

namespace Banky.Test.Modules.Clients;

public class GetAllClients_Tests
{
    private readonly IClientsRepository _repository = Substitute.For<IClientsRepository>();
    private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();

    [Fact]
    public async Task GetAllClients_ShouldReturnExpectedClientData()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        var clients = new[]
        {
            Client
                .Create(ClientType.Person, "Mark", "Thompson", "mark@gmail.com", new(1988, 5, 8), _timeProvider)
                .Value,
            Client
                .Create(ClientType.Person, "Dana", "Smith", "danasmith@gmail.com", new(2002, 8, 7), _timeProvider)
                .Value,
            Client.Create(ClientType.Company, "ACME Inc.", null, "ceo@acme.com", null, _timeProvider).Value
        };

        _repository.GetAll(Arg.Any<CancellationToken>()).Returns(clients);

        // Act
        var result = await GetAllClients.Handler(_repository, default);

        // Assert
        result.Result.Should().BeOfType<Ok<GetAllClients.Response>>();
        var okResult = (Ok<GetAllClients.Response>)result.Result;

        var resultClients = okResult!.Value?.Clients;
        resultClients.Should().NotBeNull();
        resultClients.Should().HaveSameCount(clients);

        for (int i = 0; i < clients.Length; i++)
        {
            var client = clients[i];
            var resultClient = resultClients!.ElementAt(i);

            resultClient.Id.Should().Be(client.Id);
            resultClient.Type.Should().Be(client.Type);
            resultClient.Name.Should().Be(client.Name);
            resultClient.LastName.Should().Be(client.LastName);
            resultClient.Email.Should().Be(client.Email);
            resultClient.BirthDate.Should().Be(client.BirthDate);
            resultClient.CreatedOnUtc.Should().Be(client.CreatedOnUtc);
            resultClient.UpdatedOnUtc.Should().Be(client.UpdatedOnUtc);
        }
    }
}
