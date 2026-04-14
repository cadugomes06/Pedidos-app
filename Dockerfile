FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["PedidosApp/PedidosApp.csproj", "PedidosApp/"]
RUN dotnet restore "PedidosApp/PedidosApp.csproj"

COPY PedidosApp/ PedidosApp/
RUN dotnet publish "PedidosApp/PedidosApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PedidosApp.dll"]