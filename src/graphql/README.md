# GraphQL

## endpoints

- `/playground` to try things out
- `/voyager` to look at api graph

## scaffolding

[Reverse Engineering](https://docs.microsoft.com/en-us/ef/core/managing-schemas/scaffolding)

- Install the ef cli
  `dotnet tool install --global dotnet-ef`
- scaffold the open sgid db from the `/graphql` working directory
  `dotnet ef dbcontext scaffold Name=OpenSGID Npgsql.EntityFrameworkCore.PostgreSQL --output-dir schemas --context CloudSqlContext --context-dir .`

## secrets

- create spot for local secrets
  `dotnet user-secrets init`
- set the database connection secret
  `dotnet user-secrets set "ConnectionStrings:OpenSGID" "Host=;Database=;Username=;Password=;`
