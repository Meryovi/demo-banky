FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY ./app .
ENV ASPNETCORE_URLS=http://+:80;https://+:443
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Banky.Server.dll"]
