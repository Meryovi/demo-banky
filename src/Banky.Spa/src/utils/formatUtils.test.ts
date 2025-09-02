import { describe, test, expect } from "vitest";
import { formatAmount } from "./formatUtils";

describe("formatUtils", () => {
  test("formatAmount formats numbers as USD currency", () => {
    expect(formatAmount(0)).toBe("$0.00");
    expect(formatAmount(1234.5)).toBe("$1,234.50");
    expect(formatAmount(1000000)).toBe("$1,000,000.00");
  });
});
