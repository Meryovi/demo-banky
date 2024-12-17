import createClient from "openapi-fetch";
import { paths } from "../schema";

const basePath = window && window.location.hostname === "localhost" ? "http://localhost:5177" : "";
export const client = createClient<paths>({ baseUrl: basePath });

export class ValidationError extends Error {
  constructor(public readonly errors: string[]) {
    super(errors.join(", "));
  }
}
