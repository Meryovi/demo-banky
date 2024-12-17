using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

namespace Banky.Test.Modules.Accounts;

public class UpdateAccount_Tests
{
    private readonly IAccountsRepository _accountRepository = Substitute.For<IAccountsRepository>();
    private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();

    [Fact]
    public async Task UpdateAccount_ShouldReturnExpectedResult_WhenAccountExists()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        var account = Account.Create(Guid.NewGuid(), AccountType.Savings, "Test Account", _timeProvider).Value;

        _accountRepository.GetById(account.Id, Arg.Any<CancellationToken>()).Returns(account);

        // Act
        var request = new UpdateAccount.Request("New name?");
        var response = await UpdateAccount.Handler(
            account.ClientId,
            account.Id,
            request,
            _accountRepository,
            default
        );

        // Assert
        response.Result.Should().BeOfType<Ok<UpdateAccount.Response>>();
        var okResult = (Ok<UpdateAccount.Response>)response.Result;

        var resultAccount = okResult?.Value?.Account;
        resultAccount.Should().NotBeNull();
        resultAccount!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task UpdateAccount_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        _accountRepository.GetById(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Account?)null);

        // Act
        var request = new UpdateAccount.Request("New name?");
        var response = await UpdateAccount.Handler(Guid.Empty, Guid.Empty, request, _accountRepository, default);

        // Assert
        response.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task UpdateAccount_ShouldReturnBadRequest_WhenInvalidDataIsProvided()
    {
        // Arrange
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));

        var account = Account.Create(Guid.NewGuid(), AccountType.Savings, "Test Account", _timeProvider).Value;

        _accountRepository.GetById(account.Id, Arg.Any<CancellationToken>()).Returns(account);

        // Act
        var request = new UpdateAccount.Request(string.Empty);
        var response = await UpdateAccount.Handler(
            account.ClientId,
            account.Id,
            request,
            _accountRepository,
            default
        );

        // Assert
        response.Result.Should().BeOfType<BadRequest<IEnumerable<string>>>();
        var badRequestResult = (BadRequest<IEnumerable<string>>)response.Result;

        var errors = badRequestResult?.Value;
        errors.Should().HaveCount(1);
        errors.Should().Contain(Errors.NameCannotBeEmpty().Message);
    }
}
