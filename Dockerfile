FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /build

# Install Node
RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl git nano
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash - && apt-get install -yq nodejs build-essential
RUN npm install -g npm

# Restore all layers...
COPY *.sln .
COPY *.props .
COPY src/Modules/*.props src/Modules/
COPY src/Banky.Server/*.csproj src/Banky.Server/
COPY src/Modules/Banky.Modules.Accounts/*.csproj src/Modules/Banky.Modules.Accounts/
COPY src/Modules/Banky.Modules.Clients/*.csproj src/Modules/Banky.Modules.Clients/
COPY test/*.props test/
COPY test/Banky.Test.Modules.Accounts/*.csproj test/Banky.Test.Modules.Accounts/
COPY test/Banky.Test.Modules.Clients/*.csproj test/Banky.Test.Modules.Clients/
RUN dotnet restore Banky.sln

# Copy the rest, build and publish artifacts
COPY . .
RUN dotnet publish src/Banky.Server/Banky.Server.csproj -c Release -o /output

FROM mcr.microsoft.com/dotnet/aspnet:9.0
EXPOSE 8080
WORKDIR /app
COPY --from=build /output .
ENTRYPOINT ["dotnet", "Banky.Server.dll"]
