FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ItPlanet.Domain/ItPlanet.Domain.csproj", "ItPlanet.Domain/"]
COPY ["ItPlanet.Infrastructure/ItPlanet.Infrastructure.csproj", "ItPlanet.Infrastructure/"]
COPY ["ItPlanet.Web/ItPlanet.Web.csproj", "ItPlanet.Web/"]
RUN dotnet restore "ItPlanet.Web/ItPlanet.Web.csproj"
COPY . .
WORKDIR "/src/ItPlanet.Web"
RUN dotnet build "ItPlanet.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ItPlanet.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ItPlanet.Web.dll"]
