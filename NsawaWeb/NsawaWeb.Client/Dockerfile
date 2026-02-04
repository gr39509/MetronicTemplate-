FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NsawaWeb/NsawaWeb.Client/NsawaWeb.Client.csproj", "NsawaWeb/NsawaWeb.Client/"]
RUN dotnet restore "NsawaWeb/NsawaWeb.Client/NsawaWeb.Client.csproj"
COPY . .
WORKDIR "/src/NsawaWeb/NsawaWeb.Client"
RUN dotnet build "./NsawaWeb.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NsawaWeb.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NsawaWeb.Client.dll"]
