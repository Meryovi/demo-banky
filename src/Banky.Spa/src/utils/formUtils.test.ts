import { describe, test, expect } from "vitest";
import { z } from "zod";
import { parseFormWithSchema } from "./formUtils";

describe("formUtils", () => {
  const Schema = z.object({
    accountName: z.string().min(5),
    initialBalance: z.coerce.number().min(1),
  });

  test("parseFormWithSchema returns parsed data when valid", () => {
    const fd = new FormData();
    fd.set("accountName", "My Account");
    fd.set("initialBalance", "50");

    const result = parseFormWithSchema(fd, Schema);

    expect(result.errors).toBeNull();
    expect(typeof result.data.initialBalance).toBe("number");
    expect(result.data.initialBalance).toBe(50);
  });

  test("parseFormWithSchema returns errors when invalid", () => {
    const fd = new FormData();
    fd.set("accountName", "abc"); // too short
    fd.set("initialBalance", "0"); // below min

    const result = parseFormWithSchema(fd, Schema);

    expect(Array.isArray(result.errors)).toBe(true);
    expect(result.errors!.length).toBeGreaterThan(0);
  });
});
