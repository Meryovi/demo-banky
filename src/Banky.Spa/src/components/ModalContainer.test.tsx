import { render, screen, waitFor } from "@testing-library/react";
import { describe, test, expect } from "vitest";
import ModalContainer from "./ModalContainer";

describe("ModalContainer", () => {
  test("renders children inside a dialog via portal and opens with opacity-100", async () => {
    // Act
    render(
      <ModalContainer>
        <div>My Modal Content</div>
      </ModalContainer>,
    );

    // Assert: content is rendered
    screen.getByText("My Modal Content");

    // Dialog exists and is open
    const dialog = document.querySelector("dialog");
    expect(dialog).not.toBeNull();
    expect(dialog?.getAttribute("open")).not.toBeNull();

    // Wait for the useEffect toggle to apply the "open" transition class
    await waitFor(() => {
      expect(dialog!.className).toContain("opacity-100");
    });
  });
});
