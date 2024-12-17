import { useState } from "react";
import { useAccountsQuery, useCloseAccountMutation } from "../hooks";
import { components } from "../../../schema";
import { formatAmount } from "../../../utils/formatUtils";
import { LinkButton, PrimaryButton } from "../../../components/Buttons";
import OpenAccountModal from "./OpenAccountModal";
import TransferBetweenAccountsModal from "./TransferBetweenAccountsModal";

const MAX_ACTIVE_ACCOUNTS = 4; // Would normally be a configuration or data-driven value.

export default function AccountsList(props: { clientId: string }) {
  const accounts = useAccountsList(props.clientId);

  if (accounts.isLoading || !accounts.accounts) return <div>Loading...</div>;
  if (accounts.error) return <div>There was an error fetching accounts: {accounts.error.toString()}</div>;

  return (
    <div className="flex flex-col align-middle">
      {accounts.accounts.map((account) => (
        <AccountCard
          key={account.id}
          account={account}
          onAccountTransfer={accounts.onAccountTransfer}
          onCloseAccount={accounts.onCloseAccount}
        />
      ))}
      {accounts.accounts.length === 0 && <div>You have no accounts. 😔</div>}

      {accounts.showOpenAccount && (
        <PrimaryButton onClick={accounts.onOpenAccount} className="mt-2">
          Open account
        </PrimaryButton>
      )}
      {accounts.isOpeningAccount && (
        <OpenAccountModal clientId={props.clientId} onClose={accounts.onOpenAccountClose} />
      )}
      {accounts.transferFromAccount && (
        <TransferBetweenAccountsModal
          clientId={props.clientId}
          fromAccountId={accounts.transferFromAccount}
          onClose={accounts.onAccountTransferClose}
        />
      )}
    </div>
  );
}

function AccountCard(props: {
  account: Account;
  onAccountTransfer: (account: Account) => void;
  onCloseAccount: (account: Account) => void;
}) {
  const { account, onAccountTransfer, onCloseAccount } = props;

  const displayOptions = !account.isClosed;
  const displayTransfer = account.balance !== 0;
  const displayClose = account.balance === 0;

  return (
    <div className="bg-white/85 rounded-xl drop-shadow-sm text-black p-3 flex justify-between mb-3">
      <div>
        <div className={`pb-2 font-light ${account.isClosed ? "line-through" : ""}`}>{account.name}</div>
        <div className="text-indigo-600 text-2xl font-semibold">{formatAmount(account.balance)}</div>
      </div>
      {displayOptions && (
        <div className="flex flex-col align-middle justify-around text-sm">
          {displayTransfer && <LinkButton onClick={() => onAccountTransfer(account)}>Transfer</LinkButton>}
          {displayClose && <LinkButton onClick={() => onCloseAccount(account)}>Close</LinkButton>}
        </div>
      )}
    </div>
  );
}

const useAccountsList = (clientId: string) => {
  const { accounts, isLoading, error } = useAccountsQuery(clientId);
  const { closeAccount } = useCloseAccountMutation(clientId);
  const [isOpeningAccount, setIsOpeningAccount] = useState(false);
  const [transferFromAccount, setTransferFromAccount] = useState<string | null>(null);

  const onCloseAccount = (account: Account) => closeAccount(account.id).catch((error) => window.alert(error.message));
  const onOpenAccount = () => setIsOpeningAccount(true);
  const onOpenAccountClose = () => setIsOpeningAccount(false);
  const onAccountTransfer = (account: Account) => setTransferFromAccount(account.id);
  const onAccountTransferClose = () => setTransferFromAccount(null);

  const showOpenAccount = accounts && accounts.filter((a) => !a.isClosed).length < MAX_ACTIVE_ACCOUNTS;

  return {
    accounts,
    isLoading,
    error,
    showOpenAccount,
    isOpeningAccount,
    transferFromAccount,
    onCloseAccount,
    onOpenAccount,
    onOpenAccountClose,
    onAccountTransfer,
    onAccountTransferClose,
  };
};

type Account = components["schemas"]["AccountDto"];
