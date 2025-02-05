FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY ./app .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "Banky.Server.dll"]
