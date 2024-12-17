using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

namespace Banky.Test.Modules.Clients;

public class GetClient_Tests
{
    private readonly IClientsRepository _repository = Substitute.For<IClientsRepository>();
    private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();

    [Fact]
    public async Task GetClient_ShouldReturnExpectedClientData_WhenClientExists()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        var client = Client
            .Create(ClientType.Person, "Mark", "Thompson", "mark@gmail.com", new(1988, 5, 8), _timeProvider)
            .Value;

        _repository.GetById(client.Id, Arg.Any<CancellationToken>()).Returns(client);

        // Act
        var result = await GetClient.Handler(client.Id, _repository, default);

        // Assert
        result.Result.Should().BeOfType<Ok<GetClient.Response>>();
        var okResult = (Ok<GetClient.Response>)result.Result;

        var resultClient = okResult!.Value?.Client;
        resultClient.Should().NotBeNull();

        resultClient!.Id.Should().Be(client.Id);
        resultClient.Type.Should().Be(client.Type);
        resultClient.Name.Should().Be(client.Name);
        resultClient.LastName.Should().Be(client.LastName);
        resultClient.Email.Should().Be(client.Email);
        resultClient.BirthDate.Should().Be(client.BirthDate);
        resultClient.CreatedOnUtc.Should().Be(client.CreatedOnUtc);
        resultClient.UpdatedOnUtc.Should().Be(client.UpdatedOnUtc);
    }

    [Fact]
    public async Task GetClient_ShouldReturnNotFound_WhenClientIsNotFound()
    {
        // Arrange
        _repository.GetById(default, default).ReturnsForAnyArgs((Client?)null);

        // Act
        var result = await GetClient.Handler(Guid.Empty, _repository, default);

        // Assert
        result.Result.Should().BeOfType<NotFound>();
    }
}
