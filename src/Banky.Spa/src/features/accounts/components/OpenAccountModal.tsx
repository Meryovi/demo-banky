import { useActionState } from "react";
import { z } from "zod";
import { LinkButton, PrimaryButton } from "../../../components/Buttons";
import ModalContainer from "../../../components/ModalContainer";
import { useOpenAccountMutation } from "../hooks";
import { ValidationError } from "../../httpClient";
import { parseFormWithSchema } from "../../../utils/formUtils";

const FormSchema = z.object({
  accountName: z.string().min(5),
  type: z.coerce.number().refine((value) => value === 100 || value === 200, { message: "Invalid account type" }),
  initialBalance: z.coerce.number().min(1.0),
});

interface FormState {
  data: z.infer<typeof FormSchema>;
  errors: string[];
}

export default function OpenAccountModal(props: { clientId: string; onClose: () => void }) {
  const { openAccount } = useOpenAccountMutation(props.clientId);

  const [state, openAccountAction, isPending] = useActionState<FormState, FormData>(
    async (_, formData) => {
      const { data, errors } = parseFormWithSchema(formData, FormSchema);

      if (errors) return { data, errors };

      try {
        await openAccount(data);
        props.onClose();
      } catch (err: any) {
        const issues: string[] = err instanceof ValidationError ? err.errors : [err.toString()];
        return { data, errors: issues };
      }

      return { data, errors: [] };
    },
    { data: { accountName: "", initialBalance: 0, type: 200 }, errors: [] },
  );

  return (
    <ModalContainer>
      <h2 className="text-xl font-semibold">Open new account</h2>
      <form action={openAccountAction}>
        {state.errors.map((error) => (
          <div key={error} className="text-red-700 text-sm mt-2">
            {error}
          </div>
        ))}
        <div>
          <label htmlFor="name" className="block mt-4 mb-2">
            Account name
          </label>
          <input
            id="accountName"
            name="accountName"
            type="text"
            autoComplete="off"
            className="w-full border rounded p-2"
            autoFocus
            required
            minLength={FormSchema.shape.accountName.minLength!}
            defaultValue={state.data.accountName}
          />
          <small className="text-gray-500">This will be the display name of your account.</small>
        </div>
        <div>
          <label htmlFor="type" className="block mt-4 mb-2">
            Account type
          </label>
          <select id="type" name="type" className="w-full border rounded p-2" required defaultValue={state.data.type}>
            <option value={200}>Savings</option>
            <option value={100}>Checking</option>
          </select>
        </div>
        <div>
          <label htmlFor="initialBalance" className="block mt-4 mb-2">
            Initial deposit
          </label>
          <input
            id="initialBalance"
            name="initialBalance"
            type="number"
            className="w-full border rounded p-2"
            required
            min={FormSchema.shape.initialBalance.minValue!}
            step={0.01}
            defaultValue={state.data.initialBalance}
          />
        </div>
        <div className="mt-4 flex justify-end">
          <PrimaryButton type="submit" disabled={isPending}>
            Open account
          </PrimaryButton>
          <LinkButton className="ml-4" onClick={props.onClose}>
            Cancel
          </LinkButton>
        </div>
      </form>
    </ModalContainer>
  );
}
