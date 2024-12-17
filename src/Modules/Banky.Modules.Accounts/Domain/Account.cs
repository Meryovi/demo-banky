using FluentResults;

namespace Banky.Modules.Accounts.Domain;

internal sealed class Account
{
    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public AccountType Type { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal Balance { get; private set; }
    public bool IsClosed { get; private set; }
    public DateTimeOffset CreatedOnUtc { get; private set; }
    public DateTimeOffset? ClosedOnUtc { get; private set; }

    private Account() { }

    public static Result<Account> Create(
        Guid clientId,
        AccountType type,
        string name,
        TimeProvider time,
        Guid? id = null
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            return Errors.NameCannotBeEmpty();

        return new Account()
        {
            Id = id ?? Guid.CreateVersion7(),
            ClientId = clientId,
            Type = type,
            Name = name,
            Balance = 0,
            IsClosed = false,
            CreatedOnUtc = time.GetUtcNow()
        };
    }

    public Result<Account> Deposit(decimal amount)
    {
        if (IsClosed)
            return Errors.CannotOperateOnClosedAccount(Name);

        if (amount <= 0)
            return Errors.AmountMustBeGreaterThanZero();

        Balance += amount;

        return this;
    }

    public Result<Account> Withdraw(decimal amount)
    {
        decimal overdraftLimit = Math.Max(0, Balance) * -0.2M; // Would normally be a configuration or product property...

        if (IsClosed)
            return Errors.CannotOperateOnClosedAccount(Name);

        if (amount <= 0)
            return Errors.AmountMustBeGreaterThanZero();

        if (Type == AccountType.Savings && Balance < amount)
            return Errors.InsufficientFundsInSavingsAccount(Balance);

        if (Type == AccountType.Checking && (Balance - amount) < overdraftLimit)
            return Errors.OverdraftLimitExceeded(Balance);

        Balance -= amount;

        return this;
    }

    public Result<(Account Origin, Account Destination)> TransferTo(Account destination, decimal amount)
    {
        if (destination.IsClosed)
            return Errors.CannotOperateOnClosedAccount(destination.Name);

        var withdrawResult = Withdraw(amount);

        if (withdrawResult.IsFailed)
            return withdrawResult.ToResult();

        destination.Balance += amount;

        return (this, destination);
    }

    public Result<Account> Update(string name)
    {
        if (IsClosed)
            return Errors.CannotOperateOnClosedAccount(Name);

        if (string.IsNullOrWhiteSpace(name))
            return Errors.NameCannotBeEmpty();

        Name = name;

        return this;
    }

    public Result<Account> Close(TimeProvider time)
    {
        if (IsClosed)
            return this;

        if (Math.Round(Balance, 0) != 0)
            return Errors.CannotCloseAccountWithNonZeroBalance(Balance);

        IsClosed = true;
        ClosedOnUtc = time.GetUtcNow();

        return this;
    }

    internal static Account Create(
        Guid id,
        Guid clientId,
        AccountType type,
        string name,
        decimal balance,
        DateTimeOffset createdOnUtc
    )
    {
        return new Account()
        {
            Id = id,
            ClientId = clientId,
            Type = type,
            Name = name,
            Balance = balance,
            CreatedOnUtc = createdOnUtc,
        };
    }
}
