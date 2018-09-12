FROM microsoft/dotnet:2.2.0-preview2-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2.100-preview2-sdk AS build
WORKDIR /src
COPY src/api/api.mapserv.utah.gov.csproj project/
WORKDIR /src/project
RUN dotnet restore
COPY src/api .
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "api.mapserv.utah.gov.dll"]
