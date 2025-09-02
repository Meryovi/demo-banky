import { z } from "zod";

export function parseFormWithSchema<T>(formData: FormData, schema: z.Schema<T>): ParsedFormResult<T> {
  const dataObject = Object.fromEntries(formData.entries());
  const parseResult = schema.safeParse(dataObject);

  if (!parseResult.success) {
    const errors = parseResult.error.issues.map((error) => error.message);
    return { data: dataObject, errors };
  }

  return { data: parseResult.data, errors: null };
}

type ParsedFormResult<T> =
  | {
      data: T;
      errors: null;
    }
  | {
      data: any;
      errors: string[];
    };
