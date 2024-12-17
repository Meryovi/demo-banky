## Cloning the Project

```sh
git clone https://github.com/Meryovi/demo-banky.git
```

## Running locally

To run the .NET host project (which will automatically also run the React project), run:

```sh
dotnet run --project .\src\Banky.Server\
```

Or, if you're using VS Code, run the 'dotnet:run' task.

API Explorer:

http://localhost:5177/scalar/v1

## Running the Docker container

If you would rather run the Docker containers, which will also spin up a Postgres instance, run:

```sh
docker compose up -d
```

Of course, you will need to have Docker installed.
