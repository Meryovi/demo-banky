import { afterEach, describe, expect, test, vi } from "vitest";
import { cleanup, screen, render } from "@testing-library/react";

import { testClient } from "./test/data";
import { useCurrentClientQuery } from "./features/clients/hooks";
import "./features/accounts/components/AccountsList";
import App from "./App";

describe("App", () => {
  vi.mock("./features/clients/hooks");
  vi.mock("./features/accounts/components/AccountsList", () => ({ default: () => <>[accounts-list]</> }));

  afterEach(() => {
    vi.resetAllMocks();
    cleanup();
  });

  test("renders accounts list component", () => {
    // Arrange
    const client = testClient;
    vi.mocked(useCurrentClientQuery).mockReturnValue({ client: client, isLoading: false, error: null });

    // Act
    render(<App />);

    // Assert
    expect(useCurrentClientQuery).toHaveBeenCalled();
    screen.getByText("[accounts-list]");
  });

  test("renders loading state", () => {
    // Arrange
    vi.mocked(useCurrentClientQuery).mockReturnValue({ client: undefined, isLoading: true, error: null });

    // Act
    render(<App />);

    // Assert
    expect(useCurrentClientQuery).toHaveBeenCalled();
    screen.getByText("Loading...");
  });

  test("renders error state", () => {
    // Arrange
    vi.mocked(useCurrentClientQuery).mockReturnValue({ client: undefined, isLoading: false, error: Error() });

    // Act
    render(<App />);

    // Assert
    expect(useCurrentClientQuery).toHaveBeenCalled();
    screen.getByText(/Oops!/);
  });
});
