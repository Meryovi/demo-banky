using NSubstitute;

namespace Banky.Test.Modules.Accounts;

public class Account_Domain_Tests
{
    private readonly TimeProvider _time = Substitute.For<TimeProvider>();

    public Account_Domain_Tests()
    {
        _time.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void Deposit_ShouldIncreaseBalance_WhenAmountIsPositive()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), AccountType.Savings, "Test", _time).Value;

        // Act & Assert
        account.Deposit(100).IsSuccess.Should().BeTrue();
        account.Balance.Should().Be(100);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Deposit_ShouldFail_WhenAmountIsNotPositive(decimal amount)
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), AccountType.Savings, "Test", _time).Value;

        // Act
        var result = account.Deposit(amount);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Select(e => e.Message).Should()
            .Contain(Errors.AmountMustBeGreaterThanZero().Message);
    }

    [Fact]
    public void Withdraw_Savings_ShouldFail_OnInsufficientFunds()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), AccountType.Savings, "Test", _time).Value;
        account.Deposit(50);

        // Act
        var result = account.Withdraw(100);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Select(e => e.Message).Should()
            .Contain(Errors.InsufficientFundsInSavingsAccount(account.Balance).Message);
        account.Balance.Should().Be(50);
    }

    [Fact]
    public void Withdraw_Checking_ShouldEnforceOverdraftLimit()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), AccountType.Checking, "Test", _time).Value;
        account.Deposit(100);

        // Arrange (reach overdraft range)
        account.Withdraw(110).IsSuccess.Should().BeTrue();

        // Act
        var result = account.Withdraw(15);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Select(e => e.Message).Should()
            .Contain(Errors.OverdraftLimitExceeded(account.Balance).Message);
    }

    [Fact]
    public void ClosedAccount_ShouldRejectOperations()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), AccountType.Savings, "Test", _time).Value;
        account.Close(_time);

        // Act & Assert
        account.Deposit(10).IsFailed.Should().BeTrue();
        account.Withdraw(10).IsFailed.Should().BeTrue();
    }

    [Fact]
    public void Close_ShouldFail_WhenBalanceIsNotZero()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), AccountType.Savings, "Test", _time).Value;
        account.Deposit(1);

        // Act
        var result = account.Close(_time);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Select(e => e.Message).Should()
            .Contain(Errors.CannotCloseAccountWithNonZeroBalance(account.Balance).Message);
        account.IsClosed.Should().BeFalse();
    }

    [Fact]
    public void Close_ShouldBeIdempotent()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), AccountType.Savings, "Test", _time).Value;

        // Act
        account.Close(_time).IsSuccess.Should().BeTrue();
        account.Close(_time).IsSuccess.Should().BeTrue();

        // Assert
        account.IsClosed.Should().BeTrue();
    }

    [Fact]
    public void Transfer_ShouldMoveFunds_WhenValid()
    {
        // Arrange
        var from = Account.Create(Guid.NewGuid(), AccountType.Savings, "From", _time).Value;
        var to = Account.Create(Guid.NewGuid(), AccountType.Savings, "To", _time).Value;
        from.Deposit(200);
        to.Deposit(50);

        // Act
        var result = from.TransferTo(to, 75);

        // Assert
        result.IsSuccess.Should().BeTrue();
        from.Balance.Should().Be(125);
        to.Balance.Should().Be(125);
    }

    [Fact]
    public void Transfer_ShouldFail_WhenDestinationIsClosed()
    {
        // Arrange
        var from = Account.Create(Guid.NewGuid(), AccountType.Savings, "From", _time).Value;
        var to = Account.Create(Guid.NewGuid(), AccountType.Savings, "To", _time).Value;
        from.Deposit(200);
        to.Close(_time);

        // Act
        var result = from.TransferTo(to, 10);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Select(e => e.Message).Should()
            .Contain(Errors.CannotOperateOnClosedAccount(to.Name).Message);
    }
}
