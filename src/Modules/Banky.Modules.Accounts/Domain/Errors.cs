using FluentResults;

namespace Banky.Modules.Accounts.Domain;

internal static class Errors
{
    internal static Error CannotOperateOnClosedAccount(string name) =>
        new($"Cannot operate on a closed account '{name}'.");

    internal static Error AmountMustBeGreaterThanZero() => new("Amount must be greater than zero.");

    internal static Error InsufficientFundsInSavingsAccount(decimal balance) =>
        new($"Insufficient funds in Savings account (balance {balance:N2}).");

    internal static Error CannotCloseAccountWithNonZeroBalance(decimal balance) =>
        new($"Cannot close account with non-zero balance (balance {balance:N2}).");

    internal static Error NameCannotBeEmpty() => new("Account name cannot be empty.");

    internal static Error OverdraftLimitExceeded(decimal balance) =>
        new($"Insufficient funds in Checking account (balance {balance:N2}). Overdraft limit exceeded.");
}
