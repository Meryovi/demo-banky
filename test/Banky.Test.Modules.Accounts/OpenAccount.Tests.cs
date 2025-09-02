using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

namespace Banky.Test.Modules.Accounts;

public class OpenAccount_Tests
{
    private readonly IAccountsRepository _accountsRepository = Substitute.For<IAccountsRepository>();
    private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();

    [Fact]
    public async Task OpenAccount_ShouldCreateAccount_WhenCallSucceeds()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        // Act
        var request = new OpenAccount.Request("Test Account", AccountType.Savings, 500);
        var result = await OpenAccount.Handler(
            Guid.NewGuid(),
            request,
            _accountsRepository,
            _timeProvider,
            default
        );

        // Assert
        result.Result.Should().BeOfType<Ok<OpenAccount.Response>>();
        var okResult = (Ok<OpenAccount.Response>)result.Result;

        var resultAccount = okResult?.Value?.Account;
        resultAccount.Should().NotBeNull();
        resultAccount!.Name.Should().Be(request.AccountName);
        resultAccount!.Type.Should().Be(request.Type);
        resultAccount!.Balance.Should().Be(request.InitialBalance);
    }

    [Fact]
    public async Task OpenAccount_ShouldBadRequest_WhenInvalidDataIsProvided()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        // Act
        var request = new OpenAccount.Request(string.Empty, 0, -1);
        var result = await OpenAccount.Handler(
            Guid.NewGuid(),
            request,
            _accountsRepository,
            _timeProvider,
            default
        );

        // Assert
        result.Result.Should().BeOfType<BadRequest<IEnumerable<string>>>();
        var badRequestResult = (BadRequest<IEnumerable<string>>)result.Result;

        var resultErrors = badRequestResult?.Value;
        resultErrors.Should().HaveCount(4);
        resultErrors.Should().ContainMatch("*'Account Name' must not be empty*");
        resultErrors.Should().ContainMatch("*'Account Name' must be at least*");
        resultErrors.Should().ContainMatch("*'Initial Balance' must be greater than '0'*");
    }

    [Fact]
    public async Task OpenAccount_ShouldInsert_IntoRepository()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));
        var clientId = Guid.NewGuid();
        var request = new OpenAccount.Request("Account A", AccountType.Savings, 100);

        // Act
        await OpenAccount.Handler(clientId, request, _accountsRepository, _timeProvider, default);

        // Assert
        await _accountsRepository.Received(1).Insert(Arg.Any<Account>(), Arg.Any<CancellationToken>());
    }
}
