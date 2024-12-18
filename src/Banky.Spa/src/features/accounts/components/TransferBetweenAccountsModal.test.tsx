import { afterEach, describe, expect, test, vi } from "vitest";
import { cleanup, fireEvent, render, screen } from "@testing-library/react";

import { testAccounts } from "../../../test/data";
import { useAccountsQuery, useAccountsTransferMutation } from "../hooks";
import TransferBetweenAccountsModal from "./TransferBetweenAccountsModal";

describe("TransferBetweenAccountsModal", () => {
  vi.mock("../hooks");

  afterEach(() => {
    vi.resetAllMocks();
    cleanup();
  });

  test("renders without crashing", () => {
    // Arrange
    const account = testAccounts[0];
    const accountTransfer = vi.fn();

    vi.mocked(useAccountsQuery).mockReturnValue({ accounts: [], isLoading: false, error: null });
    vi.mocked(useAccountsTransferMutation).mockReturnValue({ accountTransfer, isPending: false, error: null });

    // Act
    render(<TransferBetweenAccountsModal clientId={account.clientId} fromAccountId={account.id} onClose={() => {}} />);
  });

  test("renders loading state", () => {
    // Arrange
    const account = testAccounts[0];
    const accountTransfer = vi.fn();

    vi.mocked(useAccountsQuery).mockReturnValue({ accounts: [], isLoading: true, error: null });
    vi.mocked(useAccountsTransferMutation).mockReturnValue({ accountTransfer, isPending: false, error: null });

    // Act
    const { container } = render(
      <TransferBetweenAccountsModal clientId={account.clientId} fromAccountId={account.id} onClose={() => {}} />,
    );

    // Assert
    expect(container.innerText).toBeUndefined();
  });

  test("calls accountTransfer mutation when submitted", () => {
    // Arrange
    const accounts = testAccounts;
    const account = accounts[0];
    const destination = accounts[1];
    const accountTransfer = vi.fn();

    vi.mocked(useAccountsQuery).mockReturnValue({ accounts, isLoading: false, error: null });
    vi.mocked(useAccountsTransferMutation).mockReturnValue({ accountTransfer, isPending: false, error: null });

    const expectedRequest = { fromAccountId: account.id, toAccountId: destination.id, amount: 100 };

    // Act
    render(<TransferBetweenAccountsModal clientId={account.clientId} fromAccountId={account.id} onClose={() => {}} />);

    const destinationAccount = screen.getByLabelText("Destination");
    const amount = screen.getByLabelText("Amount");

    fireEvent.change(destinationAccount, { target: { value: expectedRequest.toAccountId } });
    fireEvent.change(amount, { target: { value: expectedRequest.amount.toString() } });
    screen.getByText("Transfer").click();

    // Assert
    expect(accountTransfer).toHaveBeenCalledWith(expectedRequest);
  });

  test("does not call accountTransfer mutation with incomplete fields", () => {
    // Arrange
    const accounts = testAccounts;
    const account = accounts[0];
    const accountTransfer = vi.fn();

    vi.mocked(useAccountsQuery).mockReturnValue({ accounts, isLoading: false, error: null });
    vi.mocked(useAccountsTransferMutation).mockReturnValue({ accountTransfer, isPending: false, error: null });

    // Act
    render(<TransferBetweenAccountsModal clientId={account.clientId} fromAccountId={account.id} onClose={() => {}} />);
    screen.getByText("Transfer").click();

    // Assert
    expect(accountTransfer).not.toHaveBeenCalled();
  });

  test("does not include origin account as transfer option", () => {
    // Arrange
    const accounts = testAccounts;
    const account = accounts[0];
    const accountTransfer = vi.fn();

    vi.mocked(useAccountsQuery).mockReturnValue({ accounts, isLoading: false, error: null });
    vi.mocked(useAccountsTransferMutation).mockReturnValue({ accountTransfer, isPending: false, error: null });

    // Act
    render(<TransferBetweenAccountsModal clientId={account.clientId} fromAccountId={account.id} onClose={() => {}} />);

    const destination = screen.getByLabelText("Destination");
    const options = Array.from(destination.querySelectorAll("option"));
    const accountOption = options.find((o) => o.value === account.id);

    // Assert
    expect(options).toHaveLength(accounts.length - 1);
    expect(accountOption).toBeUndefined();
  });

  test("calls onClose event when canceled", () => {
    // Arrange
    const accounts = testAccounts;
    const account = accounts[0];
    const onClose = vi.fn();
    const accountTransfer = vi.fn();

    vi.mocked(useAccountsQuery).mockReturnValue({ accounts, isLoading: false, error: null });
    vi.mocked(useAccountsTransferMutation).mockReturnValue({ accountTransfer, isPending: false, error: null });

    // Act
    render(<TransferBetweenAccountsModal clientId={account.clientId} fromAccountId={account.id} onClose={onClose} />);
    screen.getByText("Cancel").click();

    // Assert
    expect(onClose).toHaveBeenCalled();
  });
});
