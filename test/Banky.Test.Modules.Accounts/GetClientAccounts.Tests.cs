using NSubstitute;

namespace Banky.Test.Modules.Accounts;

public class GetClientAccounts_Tests
{
    private readonly IAccountsRepository _accountRepository = Substitute.For<IAccountsRepository>();
    private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();

    [Fact]
    public async Task GetClientAccounts_ShouldReturnExpectedClientData()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), AccountType.Savings, "Test Account", _timeProvider).Value;

        _accountRepository.GetByClient(account.ClientId, Arg.Any<CancellationToken>()).Returns([account]);

        // Act
        var result = await GetClientAccounts.Handler(account.ClientId, _accountRepository, default);

        // Assert
        var accounts = result.Value!.Accounts;
        accounts.Should().HaveCount(1);
        accounts.Should().ContainEquivalentOf(account);
    }
}
