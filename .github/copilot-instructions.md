# UGRC API repository guidance

## What this repository is

This is the UGRC API platform. The ASP.NET Core API geocodes addresses and mileposts and searches Open SGID spatial data. A Firebase/React self-service app lets UtahID users create API keys, and an Astro/Starlight site contains the documentation. Supporting systems are Firestore, BigQuery, Redis, PostgreSQL/PostGIS, and ArcGIS Server; Smocker provides local ArcGIS HTTP mocks.

This is a mixed monorepo: C#/.NET 10, JavaScript/TypeScript, Python automation, Docker Compose, and multi-language API samples. There is no repository-wide Node or Python version file. CI uses .NET SDK `10.x`, PNPM `11`, and Node `lts/*`; Firebase Functions declare Node `22`. C# uses language version `preview`.

Trust this file and the referenced project scripts first. Search only when these instructions are incomplete or contradicted by the code.

## Layout and ownership

- `src/api/`: ASP.NET Core API (`net10.0`). Feature areas are under `Features/` (Geocoding, Milepost, Searching, Information, Converting, Health); shared behavior is in `Cache/`, `Services/`, `Infrastructure/`, `Models/`, `Middleware/`, and `Formatters/`.
- `test/api.unit.tests/`: xUnit unit tests for the API. `api.mapserv.utah.gov.sln` is the main .NET solution. `libs/EsriJson/` is a referenced geometry/Esri JSON library with NUnit tests.
- `src/developer/`: React 19/Vite self-service app, Firebase Functions, Firestore rules/indexes, migrations, and emulator scripts. `src/developer/package.json` owns its scripts and PNPM workspace.
- `src/explorer/`: Astro 5/Starlight documentation site with React and Tailwind. Documentation content is in `src/content/docs/`; site configuration is `astro.config.mjs`.
- `src/data-migration/ravendb/`: .NET RavenDB-to-Firestore migration. `src/data-migration/redis/`: Python Redis RDB tooling.
- `forklift/`: Python ArcGIS locator automation; credentials are external and must not be committed. `samples/` contains C#, VB, JavaScript, PHP, Python, Ruby, and Rust clients.
- `test/smocker/`: committed YAML mocks and `mocks.sh`; `test/api.migration.tests/` is a separate Vitest/npm project comparing old and new response shapes.
- Root configuration: `Directory.Build.props` (C# preview language), `src/Directory.Build.props` (shared `net10.0`), `omnisharp.json`, `docker-compose*.yml`, `.codecov.yml`, and `.vscode/tasks.json`/`launch.json`.

Keep changes within the owning area. Preserve existing public API behavior and update focused tests when changing request/response contracts. Use conventional commits with scopes `api`, `open-api`, `developer`, or `explorer` when a commit is requested.

## Required validation

Run commands from the repository root unless a command includes `cd`. Always use the lockfile for JavaScript installs. Use the current installed versions of the required tools; local validation does not need to match CI versions.

### .NET API and libraries

The main CI gate is:

```sh
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=../../coverage.xml /p:Configuration=Release api.mapserv.utah.gov.sln --verbosity q --nologo
```

This emits existing `NU1903` high-severity vulnerability warnings for `Microsoft.OpenApi` 2.0.0 but exits successfully. The focused VS Code equivalents are `cd src/api && dotnet build`, `cd test/api.unit.tests && dotnet test`, and `cd src/data-migration/ravendb && dotnet build`. Unit tests do not require containers. Coverage can be written as lcov with `dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=../../lcov.info` from `test/api.unit.tests`.

### Developer app

```sh
cd src/developer
pnpm install --frozen-lockfile
pnpm lint
pnpm build
```

The install, ESLint, and Vite build pass with current supported local tools. `pnpm test` is the CI script, but it runs `vitest --silent --coverage --ui --open` and is interactive. For unattended local validation use `pnpm exec vitest --run` (and add `--coverage` when coverage is needed). `pnpm start` runs Firebase emulators and Vite together; `pnpm dev:firebase` starts emulators, importing `../../data/firebase`. Do not run Firebase deploy or migration commands without the required project credentials.

### Explorer site

```sh
cd src/explorer
pnpm install --frozen-lockfile
pnpm build
```

The build currently fails during static route generation because the installed `react-aria-components` imports the private `react-stately/private/autocomplete/useAutocompleteState` subpath, which its package `exports` does not expose. It also reports a missing `src/content/i18n/` directory and an empty Tailwind `content` configuration before the failure. Treat this as a known dependency/configuration issue; do not hide it by weakening validation. `pnpm start` runs the local docs server on `http://localhost:4321/`.

### Migration tests and local services

From `test/api.migration.tests`, run `npm install` followed by `npx vitest --run` (the `npm test` script is plain `vitest` and may enter watch mode). For local API work, start the developer emulators, then `docker compose -f docker-compose.yml -f docker-compose.override.yml up cache smocker`, register mocks with `cd test/smocker && bash mocks.sh`, authenticate with `gcloud auth application-default login`, and launch the VS Code `api - no launch` configuration. Expected ports are API `1337`, Firebase UI `4000`, Vite `5173`, Smocker UI `8081`, and mocked ArcGIS `6443`. ArcGIS Server itself is not containerized.

## CI and safety notes

`.github/workflows/push.api.yml` runs the .NET solution test/coverage gate and then builds/deploys `src/api/Dockerfile`. `push.developer.yml` installs with PNPM 11, runs the developer tests, builds with secret/config environment values, and deploys Firebase. `push.explorer.yml` installs/builds/deploys the docs site. Release workflows repeat these checks for API, developer, and explorer tags; `push.yml` runs Release Please on `main`. Do not attempt cloud deploys locally.

Do not commit secrets, `.env.local`, generated `dist/`, `bin/`, `obj/`, emulator state, or credentials. Existing TODOs include development BigQuery/Redis priming and planned v2 contract changes; check nearby code before altering those behaviors. The API project intentionally overrides a vulnerable transitive dependency, so do not remove that package reference or warning suppression casually.
