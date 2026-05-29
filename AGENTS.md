# AGENTS

## Repository facts that matter
- This is a .NET 10 solution (`FiscalCore.slnx`) with 4 projects under `src/` and 1 test project under `tests/`.
- Main entrypoint is `src/FiscalCore.Api/Program.cs` (ASP.NET Core Web API + JWT auth + API versioning + Hangfire dashboard).
- Layering is enforced by project references: `Api -> Application + Infrastructure`, `Infrastructure -> Application + Domain`, `Application -> Domain`.

## High-value commands
- Restore/build solution: `dotnet restore FiscalCore.slnx` then `dotnet build FiscalCore.slnx`.
- Run API: `dotnet run --project src/FiscalCore.Api/FiscalCore.Api.csproj`.
- Run tests: `dotnet test tests/FiscalCore.UnitTests/FiscalCore.UnitTests.csproj`.
- Run a single test: `dotnet test tests/FiscalCore.UnitTests/FiscalCore.UnitTests.csproj --filter "FullyQualifiedName~Namespace.Class.TestMethod"`.

## EF Core and database workflow
- EF Core tooling is configured in `src/FiscalCore.Infrastructure` (`Microsoft.EntityFrameworkCore.Tools` + design-time factory).
- Migrations live in `src/FiscalCore.Infrastructure/Persistence/Migrations`.
- When creating/updating migrations, use Infrastructure as target and API as startup project, e.g.:
  - `dotnet ef migrations add <Name> --project src/FiscalCore.Infrastructure/FiscalCore.Infrastructure.csproj --startup-project src/FiscalCore.Api/FiscalCore.Api.csproj --output-dir Persistence/Migrations`
  - `dotnet ef database update --project src/FiscalCore.Infrastructure/FiscalCore.Infrastructure.csproj --startup-project src/FiscalCore.Api/FiscalCore.Api.csproj`

## Config and runtime gotchas
- `Program.cs` validates `Encryption` and `Jwt` options at startup (`ValidateOnStart`): missing/blank values fail boot.
- API expects `ConnectionStrings:DefaultConnection` and uses SQL Server for both EF Core contexts and Hangfire storage.
- `appsettings.*.json` is gitignored; do not assume local secrets/config exist in fresh clones.
- `src/FiscalCore.Api/Properties/launchSettings.json` defines local URLs: `http://localhost:5079` and `https://localhost:7013`.

## Current testing reality
- The test project exists but currently has only a placeholder test (`tests/FiscalCore.UnitTests/UnitTest1.cs`).
- Prefer focused build + manual API verification for feature work until meaningful tests are added.
