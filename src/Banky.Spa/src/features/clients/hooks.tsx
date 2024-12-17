import { useMutation, useQuery, useQueryClient } from "react-query";
import { client } from "../httpClient";
import { components } from "../../schema";

export function useCurrentClientQuery() {
  const { data, isLoading, error } = useQuery({
    queryKey: "currentClient",
    cacheTime: 5000,
    refetchOnWindowFocus: false,
    queryFn: fetchCurrentClient,
  });

  return { client: data, isLoading, error };
}

export function useUpdateClientMutate(clientId: string) {
  const queryClient = useQueryClient();

  const { mutateAsync, isLoading, error } = useMutation({
    mutationFn: (data: components["schemas"]["Request3"]) => updateClient(clientId, data),
    onSuccess: (data) => {
      queryClient.setQueryData("currentClient", data);
    },
  });

  return { updateClient: mutateAsync, isLoading, error };
}

const fetchClients = async () => client.GET("/api/v1/clients", {}).then((response) => response.data!.clients);

const fetchCurrentClient = async () => fetchClients().then((clients) => clients[0]); // For demo purposes.

const updateClient = async (clientId: string, data: components["schemas"]["Request3"]) =>
  client
    .PUT("/api/v1/clients/{clientId}", { params: { path: { clientId } }, body: data })
    .then((response) => response.data!.client);
