import { afterEach, describe, expect, test, vi } from "vitest";
import { cleanup, fireEvent, render, screen } from "@testing-library/react";

import { testAccounts } from "../../../test/data";
import { useOpenAccountMutation } from "../hooks";
import OpenAccountModal from "./OpenAccountModal";

describe("OpenAccountModal", () => {
  vi.mock("../hooks");

  afterEach(() => {
    vi.resetAllMocks();
    cleanup();
  });

  test("renders without crashing", () => {
    // Arrange
    const openAccount = vi.fn(() => Promise.resolve(newAccount));
    vi.mocked(useOpenAccountMutation).mockReturnValue({ openAccount, isPending: false, error: null });

    // Act
    render(<OpenAccountModal clientId={newAccount.clientId} onClose={() => {}} />);
  });

  test("calls openAccount mutation when submitted", () => {
    // Arrange
    const openAccount = vi.fn(() => Promise.resolve(newAccount));

    vi.mocked(useOpenAccountMutation).mockReturnValue({ openAccount, isPending: false, error: null });

    const expectedRequest = { accountName: "Test account", type: 100, initialBalance: 500 };

    // Act
    render(<OpenAccountModal clientId={newAccount.clientId} onClose={() => {}} />);

    const name = screen.getByLabelText("Account name");
    const type = screen.getByLabelText("Account type");
    const initialBalance = screen.getByLabelText("Initial deposit");

    fireEvent.change(name, { target: { value: expectedRequest.accountName } });
    fireEvent.change(type, { target: { value: expectedRequest.type.toString() } });
    fireEvent.change(initialBalance, { target: { value: expectedRequest.initialBalance.toString() } });
    screen.getByText("Open account").click();

    // Assert
    expect(openAccount).toHaveBeenCalledWith(expectedRequest);
  });

  test("does not call openAccount mutation with incomplete fields", () => {
    // Arrange
    const openAccount = vi.fn(() => Promise.resolve(newAccount));

    vi.mocked(useOpenAccountMutation).mockReturnValue({ openAccount, isPending: false, error: null });

    // Act
    render(<OpenAccountModal clientId={newAccount.clientId} onClose={() => {}} />);
    screen.getByText("Open account").click();

    // Assert
    expect(openAccount).not.toHaveBeenCalled();
  });

  test("calls onClose event when canceled", () => {
    // Arrange
    const onClose = vi.fn();
    const openAccount = vi.fn(() => Promise.resolve(newAccount));

    vi.mocked(useOpenAccountMutation).mockReturnValue({ openAccount, isPending: false, error: null });

    // Act
    render(<OpenAccountModal clientId={newAccount.clientId} onClose={onClose} />);
    screen.getByText("Cancel").click();

    // Assert
    expect(onClose).toHaveBeenCalled();
  });
});

const newAccount = { ...testAccounts[2], id: "3", balance: 100, isClosed: false, closedOnUtc: null };
