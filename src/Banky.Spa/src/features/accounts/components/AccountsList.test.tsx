import { afterEach, describe, expect, test, vi } from "vitest";
import { cleanup, fireEvent, render, screen } from "@testing-library/react";

import { testAccounts } from "../../../test/data";
import { useAccountsQuery, useCloseAccountMutation } from "../hooks";
import AccountsList from "./AccountsList";

describe("AccountsList", () => {
  vi.mock("../hooks");

  afterEach(() => {
    vi.resetAllMocks();
    cleanup();
  });

  test("renders without crashing", () => {
    // Arrange
    const account = testAccounts[0];
    const closeAccount = vi.fn(() => Promise.resolve(account));

    vi.mocked(useAccountsQuery).mockReturnValueOnce({ accounts: [], isLoading: false, error: null });
    vi.mocked(useCloseAccountMutation).mockReturnValueOnce({ closeAccount, isPending: false, error: null });

    // Act
    render(<AccountsList clientId={account.clientId} />);

    // Assert
    expect(useAccountsQuery).toHaveBeenCalled();
  });

  test("renders expected accounts", () => {
    // Arrange
    const accounts = testAccounts;
    const account = testAccounts[0];
    const closeAccount = vi.fn(() => Promise.resolve(account));

    vi.mocked(useAccountsQuery).mockReturnValueOnce({ accounts, isLoading: false, error: null });
    vi.mocked(useCloseAccountMutation).mockReturnValueOnce({ closeAccount, isPending: false, error: null });

    // Act
    render(<AccountsList clientId={account.clientId} />);

    // Assert
    for (const account of accounts) {
      screen.getByText(account.name);
    }
  });

  test("closes account when clicked", () => {
    // Arrange
    const accounts = testAccounts.map((a) => ({ ...a, balance: 0, isClosed: false }));
    const account = accounts[0];
    const closeAccount = vi.fn(() => Promise.resolve(account));

    vi.mocked(useAccountsQuery).mockReturnValueOnce({ accounts, isLoading: false, error: null });
    vi.mocked(useCloseAccountMutation).mockReturnValueOnce({ closeAccount, isPending: false, error: null });

    // Act
    render(<AccountsList clientId={account.clientId} />);

    const closeButton = screen.getAllByText("Close")[0];
    fireEvent.click(closeButton);

    // Assert
    expect(closeAccount).toHaveBeenCalled();
  });
});
