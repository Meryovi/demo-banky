import { useCurrentClientQuery } from "./features/clients/hooks";
import AccountsList from "./features/accounts/components/AccountsList";

function App() {
  const { client, isLoading, error } = useCurrentClientQuery();

  return (
    <div className="bg-linear-to-tr/oklch from-indigo-700 to-indigo-950 flex flex-col justify-center items-center min-h-dvh text-gray-300">
      <h1 className="text-2xl font-extralight my-2 text-gray-300">banky &bull; micro banking</h1>
      <div className="bg-black/10 p-8 rounded-xl my-4 min-w-xl">
        {client && <AccountsList clientId={client.id} />}
        {isLoading && <div>Loading...</div>}
        {!!error && <div>Oops! Failed to fetch client data.</div>}
      </div>
    </div>
  );
}

export default App;
