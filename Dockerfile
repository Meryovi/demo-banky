FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY ./app .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Banky.Server.dll"]
