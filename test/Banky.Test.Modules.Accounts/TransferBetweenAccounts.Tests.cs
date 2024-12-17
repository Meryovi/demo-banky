using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

namespace Banky.Test.Modules.Accounts;

public class TransferBetweenAccounts_Tests
{
    private readonly IAccountsRepository _accountRepository = Substitute.For<IAccountsRepository>();
    private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();

    [Fact]
    public async Task TransferBetweenAccounts_ShouldTransfer_WhenCallSucceeds()
    {
        // Arrange
        var fromAccount = Account.Create(Guid.NewGuid(), AccountType.Savings, "From Account", _timeProvider).Value;
        var toAccount = Account.Create(Guid.NewGuid(), AccountType.Savings, "To Account", _timeProvider).Value;

        fromAccount.Deposit(1000);
        toAccount.Deposit(1000);

        _accountRepository.GetById(fromAccount.Id, Arg.Any<CancellationToken>()).Returns(fromAccount);
        _accountRepository.GetById(toAccount.Id, Arg.Any<CancellationToken>()).Returns(toAccount);

        // Act
        var request = new TransferBetweenAccounts.Request(toAccount.Id, 100);
        var result = await TransferBetweenAccounts.Handler(
            fromAccount.ClientId,
            fromAccount.Id,
            request,
            _accountRepository,
            default
        );

        // Assert
        result.Result.Should().BeOfType<Ok<TransferBetweenAccounts.Response>>();
        var okResult = (Ok<TransferBetweenAccounts.Response>)result.Result;

        var resultFromAccount = okResult?.Value?.Account;
        resultFromAccount.Should().NotBeNull();
        resultFromAccount!.Balance.Should().Be(900);
    }

    [Fact]
    public async Task TransferBetweenAccounts_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        _accountRepository.GetById(default, default).ReturnsForAnyArgs((Account?)null);

        // Act
        var request = new TransferBetweenAccounts.Request(Guid.NewGuid(), 100);
        var result = await TransferBetweenAccounts.Handler(
            Guid.NewGuid(),
            Guid.NewGuid(),
            request,
            _accountRepository,
            default
        );

        // Assert
        result.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task TransferBetweenAccounts_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var fromAccount = Account.Create(Guid.NewGuid(), AccountType.Savings, "From Account", _timeProvider).Value;
        fromAccount.Deposit(1000);

        _accountRepository.GetById(fromAccount.Id, Arg.Any<CancellationToken>()).Returns(fromAccount);

        // Act
        var request = new TransferBetweenAccounts.Request(fromAccount.Id, -1);
        var result = await TransferBetweenAccounts.Handler(
            fromAccount.ClientId,
            fromAccount.Id,
            request,
            _accountRepository,
            default
        );

        // Assert
        result.Result.Should().BeOfType<BadRequest<IEnumerable<string>>>();
        var badRequestResult = (BadRequest<IEnumerable<string>>)result.Result;

        var resultErrors = badRequestResult?.Value;
        resultErrors.Should().HaveCount(2);
        resultErrors.Should().ContainMatch("*'Amount' must be greater than '0'*");
        resultErrors.Should().ContainMatch("*Cannot transfer to the same account*");
    }
}
