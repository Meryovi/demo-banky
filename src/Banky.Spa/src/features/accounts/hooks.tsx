import { useMutation, useQuery, useQueryClient } from "react-query";
import { client, ValidationError } from "../httpClient";
import { components } from "../../schema";

export const useAccountsQuery = (clientId: string) => {
  const { data, isLoading, error } = useQuery({
    queryKey: ["accounts", clientId],
    refetchOnWindowFocus: false,
    keepPreviousData: true,
    queryFn: () => fetchAccounts(clientId),
  });

  return { accounts: data, isLoading, error };
};

export function useOpenAccountMutation(clientId: string) {
  const queryClient = useQueryClient();

  const { mutateAsync, isLoading, error } = useMutation({
    mutationFn: (data: components["schemas"]["Request"]) => createAccount(clientId, data),
    onSuccess: (data) => {
      queryClient.setQueryData(["accounts", clientId], (old: unknown) =>
        Array.isArray(old) ? [...old, data] : [data],
      );
    },
  });

  return { openAccount: mutateAsync, isLoading, error };
}

export function useAccountsTransferMutation(clientId: string) {
  const queryClient = useQueryClient();

  const { mutateAsync, isLoading, error } = useMutation({
    mutationFn: (data: components["schemas"]["Request2"]) =>
      transferBetweenAccounts(clientId, data.fromAccountId ?? "", data),
    onSuccess: () => {
      queryClient.removeQueries(["accounts", clientId]);
    },
  });

  return { accountTransfer: mutateAsync, isLoading, error };
}

export function useCloseAccountMutation(clientId: string) {
  const queryClient = useQueryClient();

  const { mutateAsync, isLoading, error } = useMutation({
    mutationFn: (accountId: string) => closeAccount(clientId, accountId),
    onMutate: (accountId) => {
      // Optimistically update the local cache.
      queryClient.setQueryData(["accounts", clientId], (old: unknown) =>
        Array.isArray(old)
          ? old.map((a) => (a.id == accountId ? { ...a, isClosed: true } : a)).sort((a, b) => a.isClosed - b.isClosed)
          : old,
      );
    },
    onError: (_, accountId) => {
      // Rollback the optimistic update on error.
      queryClient.setQueryData(["accounts", clientId], (old: unknown) =>
        Array.isArray(old)
          ? old.map((a) => (a.id == accountId ? { ...a, isClosed: false } : a)).sort((a, b) => a.isClosed - b.isClosed)
          : old,
      );
    },
    onSuccess: (data) => {
      queryClient.setQueryData(["accounts", clientId], (old: unknown) =>
        Array.isArray(old) ? old.map((a) => (a.id === data.id ? data : a)) : old,
      );
    },
  });

  return { closeAccount: mutateAsync, isLoading, error };
}

const fetchAccounts = async (clientId: string) =>
  client
    .GET("/api/v1/accounts/{clientId}", { params: { path: { clientId } } })
    .then((response) => response.data!.accounts);

const createAccount = async (clientId: string, data: components["schemas"]["Request"]) =>
  client
    .POST("/api/v1/accounts/{clientId}", {
      params: { path: { clientId } },
      body: data,
    })
    .then((response) => {
      if (response.response.status === 200) {
        return response.data!.account;
      }
      throw new ValidationError(response.error!);
    });

const transferBetweenAccounts = async (clientId: string, accountId: string, data: components["schemas"]["Request2"]) =>
  client
    .POST("/api/v1/accounts/{clientId}/{accountId}/transfer", {
      params: { path: { clientId, accountId } },
      body: data,
    })
    .then((response) => {
      if (response.response.status === 200) {
        return response.data!.account;
      }
      throw new ValidationError(response.error!);
    });

const closeAccount = async (clientId: string, accountId: string) =>
  client
    .POST("/api/v1/accounts/{clientId}/{accountId}/close", { params: { path: { clientId, accountId } } })
    .then((response) => {
      if (response.response.status === 200) {
        return response.data!.account;
      }
      throw new ValidationError(response.error!);
    });
