FROM microsoft/dotnet:2.2.0-preview1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2.100-preview1-sdk AS build
WORKDIR /src
COPY src/developer/developer.mapserv.utah.gov.csproj project/
WORKDIR /src/project
RUN dotnet restore
COPY src/developer .
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "developer.mapserv.utah.gov.dll"]
