using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

namespace Banky.Test.Modules.Accounts;

public class CloseAccount_Tests
{
    private readonly IAccountsRepository _repository = Substitute.For<IAccountsRepository>();
    private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();

    [Fact]
    public async Task CloseAccount_ShouldReturnExpectedResult_WhenAccountExists()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        var account = Account.Create(Guid.NewGuid(), AccountType.Savings, "Test Account", _timeProvider).Value;

        _repository.GetById(account.Id, Arg.Any<CancellationToken>()).Returns(account);

        // Act
        var result = await CloseAccount.Handler(account.ClientId, account.Id, _repository, _timeProvider, default);

        // Assert
        result.Result.Should().BeOfType<Ok<CloseAccount.Response>>();
        var okResult = (Ok<CloseAccount.Response>)result.Result;

        var resultAccount = okResult?.Value?.Account;

        resultAccount.Should().NotBeNull();
        resultAccount!.IsClosed.Should().BeTrue();
    }

    [Fact]
    public async Task CloseAccount_ShouldReturnNotFound_WhenAccountIsNotFound()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        _repository.GetById(default, default).ReturnsForAnyArgs((Account?)null);

        // Act
        var result = await CloseAccount.Handler(Guid.Empty, Guid.Empty, _repository, _timeProvider, default);

        // Assert
        result.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task CloseAccount_ShouldSucceed_WhenAccountIsCanceled()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        var account = Account.Create(Guid.NewGuid(), AccountType.Savings, "Test Account", _timeProvider).Value;
        account.Close(_timeProvider);

        _repository.GetById(account.Id, Arg.Any<CancellationToken>()).Returns(account);

        // Act
        var result = await CloseAccount.Handler(account.ClientId, account.Id, _repository, _timeProvider, default);

        // Assert
        result.Result.Should().BeOfType<Ok<CloseAccount.Response>>();
    }

    [Fact]
    public async Task CloseAccount_ShouldReturnBadRequest_WhenAccountHasBalance()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        var account = Account.Create(Guid.NewGuid(), AccountType.Savings, "Test Account", _timeProvider).Value;
        account.Deposit(5000);

        _repository.GetById(account.Id, Arg.Any<CancellationToken>()).Returns(account);

        // Act
        var result = await CloseAccount.Handler(account.ClientId, account.Id, _repository, _timeProvider, default);

        // Assert
        result.Result.Should().BeOfType<BadRequest<IEnumerable<string>>>();
        var badRequest = (BadRequest<IEnumerable<string>>)result.Result;

        badRequest.Value.Should().NotBeEmpty();
        badRequest.Value.Should().Contain(Errors.CannotCloseAccountWithNonZeroBalance(account.Balance).Message);
    }
}
