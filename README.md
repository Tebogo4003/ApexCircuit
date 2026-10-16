# ApexCircuit / RaceDay — Combined Project

This folder combines **Part 1 (Apex Predator Circuit documentation and SQL design)** with **Part 2 (ASP.NET Core Web API and tests)**.

## Project structure

- `RaceDay.sln` — open this solution in Visual Studio.
- `RaceDay.API/` — .NET 8 Web API implementation.
- `RaceDay.Tests/` — automated tests.
- `docs/Part1/` — Part 1 README materials, ERD, SQL script, API endpoint plan, and assignment PDFs.
- `.github/workflows/ci.yml` — Part 2 CI workflow.

## Open and run in Visual Studio

1. Extract this ZIP to a normal folder.
2. Open `RaceDay.sln` in Visual Studio 2022 or newer with the **ASP.NET and web development** workload installed.
3. In `RaceDay.API/appsettings.json`, update `ConnectionStrings:DefaultConnection` for your SQL Server instance.
4. Restore NuGet packages, then build the solution.
5. Set `RaceDay.API` as the startup project and run it.
6. In Development, open `/swagger` on the launch URL shown by Visual Studio.

## Database notes

Part 1's SQL design is preserved at `docs/Part1/docs/ApexCircuit.sql`. It creates a database named `ApexCircuitDB` with Roles, Users, Events, Categories, Enrolments, and Results. Part 2 uses Entity Framework Core Code-First and the connection string in `RaceDay.API/appsettings.json`. **Do not run the Part 1 SQL script and EF migrations against the same database without first checking the schema**; they represent two ways of defining the database and may conflict.

## Important integration note

Part 1's ZIP contains project documentation and SQL assets, not a separate web application source project. Part 2 supplies the runnable API and test solution, so the combined package keeps Part 2's code intact and includes Part 1's assets without overwriting files. The API implementation and SQL schema should still be checked together before submission or deployment.
