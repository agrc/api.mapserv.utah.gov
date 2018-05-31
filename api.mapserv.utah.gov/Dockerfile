FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY api.mapserv.utah.gov/api.mapserv.utah.gov.csproj api.mapserv.utah.gov/
RUN dotnet restore api.mapserv.utah.gov/api.mapserv.utah.gov.csproj
COPY . .
WORKDIR /src/api.mapserv.utah.gov
RUN dotnet build api.mapserv.utah.gov.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish api.mapserv.utah.gov.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "api.mapserv.utah.gov.dll"]
