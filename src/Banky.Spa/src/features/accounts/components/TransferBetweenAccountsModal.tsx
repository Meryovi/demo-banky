import { useActionState } from "react";
import { z } from "zod";
import { LinkButton, PrimaryButton } from "../../../components/Buttons";
import ModalContainer from "../../../components/ModalContainer";
import { useAccountsQuery, useAccountsTransferMutation } from "../hooks";
import { formatAmount } from "../../../utils/formatUtils";
import { ValidationError } from "../../httpClient";
import { parseFormWithSchema } from "../../../utils/formUtils";

const FormSchema = z.object({
  toAccountId: z.string(),
  fromAccountId: z.string(),
  amount: z.coerce.number().min(1.0),
});

interface FormState {
  data: z.infer<typeof FormSchema>;
  errors: string[];
}

export default function TransferBetweenAccountsModal(props: {
  clientId: string;
  fromAccountId: string;
  onClose: () => void;
}) {
  const { accounts, isLoading } = useAccountsQuery(props.clientId);
  const { accountTransfer } = useAccountsTransferMutation(props.clientId);

  const fromAccount = accounts?.find((a) => a.id === props.fromAccountId);
  const destinationAccounts = accounts?.filter((a) => a.id !== props.fromAccountId && !a.isClosed);

  const [state, transferAction, isPending] = useActionState<FormState, FormData>(
    async (_, formData) => {
      const { data, errors } = parseFormWithSchema(formData, FormSchema);

      if (errors) return { data, errors };

      try {
        await accountTransfer(data);
        props.onClose();
      } catch (err: any) {
        const issues: string[] = err instanceof ValidationError ? err.errors : [err.toString()];
        return { data, errors: issues };
      }

      return { data, errors: [] };
    },
    { data: { amount: 0, fromAccountId: props.fromAccountId, toAccountId: "" }, errors: [] },
  );

  if (isLoading || !fromAccount || !destinationAccounts) return null;

  return (
    <ModalContainer>
      <h2 className="text-xl font-semibold">Open new account</h2>
      <form
        onSubmit={(e) => {
          // Not using form action to prevent the "form reset" behavior...
          e.preventDefault();
          transferAction(new FormData(e.currentTarget));
        }}>
        {state.errors.map((error) => (
          <div key={error} className="text-red-700 text-sm mt-2">
            {error}
          </div>
        ))}
        <div>
          <label htmlFor="name" className="block mt-4 mb-2">
            Origin account
          </label>
          <select
            id="fromAccountId"
            name="fromAccountId"
            className="w-full border rounded p-2"
            required
            defaultValue={state.data.fromAccountId}>
            <option value={fromAccount?.id}>
              {fromAccount?.name} - {formatAmount(fromAccount.balance)}
            </option>
          </select>
        </div>
        <div>
          <label htmlFor="toAccountId" className="block mt-4 mb-2">
            Destination
          </label>
          <select
            id="toAccountId"
            name="toAccountId"
            className="w-full border rounded p-2"
            required
            autoFocus
            defaultValue={state.data.toAccountId}>
            <option value="">Select...</option>
            {destinationAccounts.map((account) => (
              <option key={account.id} value={account.id}>
                {account.name} - {formatAmount(account.balance)}
              </option>
            ))}
          </select>
        </div>
        <div>
          <label htmlFor="amount" className="block mt-4 mb-2">
            Amount
          </label>
          <input
            id="amount"
            name="amount"
            type="number"
            className="w-full border rounded p-2"
            required
            min={FormSchema.shape.amount.minValue!}
            step={0.01}
            defaultValue={state.data.amount}
          />
        </div>
        <div className="mt-4 flex justify-end">
          <PrimaryButton type="submit" disabled={isPending}>
            Transfer
          </PrimaryButton>
          <LinkButton className="ml-4" onClick={props.onClose}>
            Cancel
          </LinkButton>
        </div>
      </form>
    </ModalContainer>
  );
}
