FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /project

COPY . ./
WORKDIR /project/src/api
RUN dotnet restore
RUN dotnet build --configuration Release --output /app

FROM build AS publish
RUN dotnet publish --configuration Release --output /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
RUN apt update -y && apt install -y libc-dev

RUN useradd -s /bin/bash dummy
USER dummy

ENTRYPOINT ["dotnet", "api.mapserv.utah.gov.dll"]
