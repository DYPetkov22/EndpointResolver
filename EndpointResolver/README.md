# EndpointResolver (.NET 9 Web API)

This is an upgrade of the original **EndpointResolver** console project, with the same
ASP.NET Core Web API features and data model as the uploaded **GlassV-2627** sample,
retargeted to **.NET 9** and **Entity Framework Core 9**.
The original reflection-based endpoint resolver is retained and can still be run.

## Requirements

- **.NET 9 SDK** (this edition targets `net9.0` even though GlassV-2627 targets .NET 10)
- **SQL Server Express LocalDB** on Windows, or another SQL Server instance
- JetBrains Rider or Visual Studio

## Opening and running in Rider

1. Open `EndpointResolver.sln` in Rider.
2. Restore NuGet packages and select the `EndpointResolver` startup configuration.
   If you see `NETSDK1045`, select the .NET 9 CLI in Rider's
   **Settings > Build, Execution, Deployment > Toolset and Build > .NET CLI executable path**.
   On this machine it is `C:\Users\PC\.dotnet\dotnet.exe`; the machine-wide CLI uses .NET 8.
3. Make sure the `DefaultConnection` string in `EndpointResolver/appsettings.json`
   points at your SQL Server instance. The default is `(localdb)\MSSQLLocalDB`
   with a database named `SchoolDb`, matching the source project.
4. In Development, startup creates the database and applies the included EF Core
   migrations automatically. SQL Server LocalDB must be installed and available.
   To apply migrations manually (for example, outside Development), run:

   ```powershell
   dotnet tool install --global dotnet-ef --version 9.0.19
   dotnet ef database update --project EndpointResolver/EndpointResolver.csproj
   ```

   If `dotnet-ef` is already installed, use `dotnet tool update --global dotnet-ef --version 9.0.19` instead.
5. Run the Web API via Rider's `http` launch profile, or run:

   ```powershell
   dotnet run --project EndpointResolver/EndpointResolver.csproj --launch-profile http
   ```

   If the default `dotnet` command uses an older SDK, use `.\run.cmd` from PowerShell.
   This launcher checks the user-profile installation for the .NET 9 SDK.
   Open `http://localhost:5187/` for the landing page. Rider's launch profiles open
   the browser automatically; `dotnet run` and the script print the URL to open.
   Use `.\run.cmd --launch-profile https` to select HTTPS instead.
   Stop the running app with Ctrl+C (or Rider's Stop button) before rebuilding,
   because Windows locks the running executable.

## Matching GlassV-2627 API routes

| HTTP | Route | Purpose |
| --- | --- | --- |
| GET | `/api/Students/GetAll` | List students |
| GET | `/api/Students/GetOne?id=1` | Find student |
| POST | `/api/Students/Create` | Create student |
| PUT | `/api/Students/Update` | Update student |
| DELETE | `/api/Students/Delete?id=1` | Delete student |
| PATCH | `/api/Students/UpdateGrade` | Update grade |

A ready-to-use set of requests is in `EndpointResolver/EndpointResolver.http`.

Student names and usernames must contain non-whitespace text; age and student IDs
must be positive integers. Grades must be nonnegative (`0` means ungraded).
Invalid requests return HTTP 400 with validation details; valid IDs that do not
exist return HTTP 404. Updates require an `id` in the JSON body.

- OpenAPI JSON: `http://localhost:5187/openapi/v1.json` (Development only).
- The **Teacher model and Teachers table** are included, as in GlassV-2627;
  that reference project does not contain a TeachersController.
- Migrations and the SQL Server provider preserve the schema from the reference
  project; their model metadata was retargeted to EF Core 9. The migrations create the
  Students and Teachers tables.
- The example contains no authentication. Student `Password` is a placeholder
  model field, not a production login implementation; never store real passwords in plaintext.

## Run the original endpoint resolver (console mode)

The custom `[Route]`, `[HttpGet]`, `WatchController`, `GameController`,
reflection-based endpoint registry, and URL/query-string parameter resolver are preserved.
They do **not** start a web server; they call the matching method in-process.

```powershell
dotnet run --project EndpointResolver/EndpointResolver.csproj -- --demo
```

Or supply a specific URL:

```powershell
dotnet run --project EndpointResolver/EndpointResolver.csproj -- --demo "https://localhost/api/Watch/PlayVideo?id=42"
```

The new ASP.NET Core Students endpoints are actual HTTP endpoints, unlike the
original reflection-based console demo.
Invalid demo URLs, missing parameters, and invalid values print a concise error
and exit with code 1.

## Build check

Run `dotnet restore` and `dotnet build` from the solution folder to check locally.
Use the .NET 9 CLI selected above; a .NET 8 SDK cannot build this project.

**Important:** This is a real ASP.NET Core **Web API** (`Microsoft.NET.Sdk.Web`)
with HTTP routes in `StudentsController`. The older reflection resolver is
still available separately with `--demo`.
