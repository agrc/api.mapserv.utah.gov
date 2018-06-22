FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY developer.mapserv.utah.gov/developer.mapserv.utah.gov.csproj developer.mapserv.utah.gov/
RUN dotnet restore developer.mapserv.utah.gov/developer.mapserv.utah.gov.csproj
COPY . .
WORKDIR /src/developer.mapserv.utah.gov
RUN dotnet build developer.mapserv.utah.gov.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish developer.mapserv.utah.gov.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "developer.mapserv.utah.gov.dll"]
