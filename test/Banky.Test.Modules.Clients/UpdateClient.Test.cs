using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

namespace Banky.Test.Modules.Clients;

public class UpdateClient_Test
{
    private readonly IClientsRepository _repository = Substitute.For<IClientsRepository>();
    private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();

    [Fact]
    public async Task UpdateClient_ShouldUpdateClientData_WhenClientExists()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        var client = Client
            .Create(ClientType.Person, "John", "Doe", "john.doe@gmail.com", new(1988, 5, 8), _timeProvider)
            .Value;

        _repository.GetById(client.Id, Arg.Any<CancellationToken>()).Returns(client);

        // Act
        var request = new UpdateClient.Request("Johnny", "Doette", "johnny@doette.com", new(1988, 5, 9));
        var result = await UpdateClient.Handler(client.Id, request, _repository, _timeProvider, default);

        // Assert
        result.Result.Should().BeOfType<Ok<UpdateClient.Response>>();
        var okResult = (Ok<UpdateClient.Response>)result.Result;

        var resultClient = okResult!.Value?.Client;
        resultClient.Should().NotBeNull();

        client.Name.Should().Be(request.Name);
        client.LastName.Should().Be(request.LastName);
        client.Email.Should().Be(request.Email);
        client.BirthDate.Should().Be(request.BirthDate);
        client.UpdatedOnUtc.Should().Be(_timeProvider.GetUtcNow());
    }

    [Fact]
    public async Task UpdateClient_ShouldReturnUpdatedData_WhenClientExists()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        var client = Client
            .Create(ClientType.Person, "John", "Doe", "john.doe@gmail.com", new(1988, 5, 8), _timeProvider)
            .Value;

        _repository.GetById(client.Id, Arg.Any<CancellationToken>()).Returns(client);

        // Act
        var request = new UpdateClient.Request("Johnny", "Doette", "johnny@doette.com", new(1988, 5, 9));
        var result = await UpdateClient.Handler(client.Id, request, _repository, _timeProvider, default);

        // Assert
        result.Result.Should().BeOfType<Ok<UpdateClient.Response>>();
        var okResult = (Ok<UpdateClient.Response>)result.Result;

        var resultClient = okResult!.Value?.Client;
        resultClient.Should().NotBeNull();

        resultClient!.Id.Should().Be(client.Id);
        resultClient.Type.Should().Be(client.Type);
        resultClient.Name.Should().Be(request.Name);
        resultClient.LastName.Should().Be(request.LastName);
        resultClient.Email.Should().Be(request.Email);
        resultClient.BirthDate.Should().Be(request.BirthDate);
        resultClient.UpdatedOnUtc.Should().Be(_timeProvider.GetUtcNow());
    }

    [Fact]
    public async Task UpdateClient_ShouldReturnNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        _repository.GetById(default, default).ReturnsForAnyArgs((Client?)null);

        // Act
        var request = new UpdateClient.Request("Johnny", "Doette", "johnny@doette.com", new(1988, 5, 9));
        var result = await UpdateClient.Handler(Guid.Empty, request, _repository, _timeProvider, default);

        // Assert
        result.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task UpdateClient_ShouldReturnBadRequest_WhenInvalidDataIsProvided()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        var client = Client
            .Create(ClientType.Person, "John", "Doe", "john.doe@gmail.com", new(1988, 5, 8), _timeProvider)
            .Value;

        _repository.GetById(client.Id, Arg.Any<CancellationToken>()).Returns(client);

        // Act
        var request = new UpdateClient.Request("Johnny", null, "johnny@doette.com", null);
        var result = await UpdateClient.Handler(client.Id, request, _repository, _timeProvider, default);

        // Assert
        result.Result.Should().BeOfType<BadRequest<IEnumerable<string>>>();
        var badRequest = (BadRequest<IEnumerable<string>>)result.Result;

        var errors = badRequest!.Value!.ToList();
        errors.Should().HaveCount(2);
        errors.Should().Contain(Errors.LastNameIsRequired().Message);
        errors.Should().Contain(Errors.BirthDateIsRequiredForPersonClients().Message);
    }

    [Fact]
    public async Task UpdateClient_ShouldReturnBadRequest_WhenUnderageIsProvided()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        var client = Client
            .Create(ClientType.Person, "John", "Doe", "john.doe@gmail.com", new(1988, 5, 8), _timeProvider)
            .Value;

        _repository.GetById(client.Id, Arg.Any<CancellationToken>()).Returns(client);

        // Act
        var request = new UpdateClient.Request(
            "Johnny",
            "Doette",
            "johnny@doette.com",
            _timeProvider.GetUtcNow().DateTime.AddYears(-17)
        );
        var result = await UpdateClient.Handler(client.Id, request, _repository, _timeProvider, default);

        // Assert
        result.Result.Should().BeOfType<BadRequest<IEnumerable<string>>>();
        var badRequest = (BadRequest<IEnumerable<string>>)result.Result;

        var errors = badRequest!.Value!.ToList();
        errors.Should().HaveCount(1);
        errors.Should().Contain(Errors.MustBeOfLegalAge(Client.PersonLegalAge).Message);
    }

    [Fact]
    public async Task UpdateClient_ShouldReturnBadRequest_WhenEmailIsInvalid()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        var client = Client
            .Create(ClientType.Person, "John", "Doe", "john.doe@gmail.com", new(1988, 5, 8), _timeProvider)
            .Value;

        _repository.GetById(client.Id, Arg.Any<CancellationToken>()).Returns(client);

        // Act
        var request = new UpdateClient.Request("Johnny", "Doette", "johnnydoette.com", new(1988, 5, 8));
        var result = await UpdateClient.Handler(client.Id, request, _repository, _timeProvider, default);

        // Assert
        result.Result.Should().BeOfType<BadRequest<IEnumerable<string>>>();
        var badRequest = (BadRequest<IEnumerable<string>>)result.Result;

        var errors = badRequest!.Value!.ToList();
        errors.Should().HaveCount(1);
        errors.Should().AllSatisfy(m => m.Should().Contain("email"));
    }
}
