import { render, screen, fireEvent } from "@testing-library/react";
import { describe, test, expect, vi } from "vitest";
import { LinkButton, PrimaryButton } from "./Buttons";

describe("Buttons", () => {
  test("PrimaryButton renders children, default type and merges classes", () => {
    // Act
    render(
      <PrimaryButton className="custom-class" onClick={() => {}}>
        Click Me
      </PrimaryButton>,
    );

    // Assert
    const button = screen.getByText("Click Me");
    expect((button as HTMLButtonElement).type).toBe("button");
    expect(button.className).toContain("bg-indigo-700");
    expect(button.className).toContain("custom-class");
  });

  test("LinkButton renders and triggers onClick", () => {
    // Arrange
    const onClick = vi.fn();

    // Act
    render(<LinkButton onClick={onClick}>Go</LinkButton>);
    const button = screen.getByText("Go");
    fireEvent.click(button);

    // Assert
    expect(onClick).toHaveBeenCalled();
    expect(button.className).toContain("text-blue-700");
  });

  test("PrimaryButton allows overriding type via props", () => {
    // Act
    render(<PrimaryButton type="submit">Submit</PrimaryButton>);

    // Assert
    const button = screen.getByText("Submit") as HTMLButtonElement;
    expect(button.type).toBe("submit");
  });
});
