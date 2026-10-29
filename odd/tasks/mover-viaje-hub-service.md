# Feature: Move ViajeHubService to MundoRuta.Servicios

## Objective

Move the SignalR client wrapper out of `MundoRuta.Server.Client` into `MundoRuta.Servicios`, decoupling it from Blazor and leaving the client service layer ready for future UI work.

## Problem

The wrapper was added in commit `f4c9c7e` inside the Blazor project and depends on `NavigationManager`. It has no consumers, mixes connection infrastructure with UI, and declares `Microsoft.AspNetCore.SignalR.Client` in the wrong project.

## Why

`MundoRuta.Servicios` is the client-side service layer (referenced only by `MundoRuta.Server.Client`). The wrapper belongs there, without Blazor dependencies, behind an interface and registered through DI.

## Scope

- Create `MundoRuta.Servicios/Realtime/IViajeHubService.cs`.
- Create `MundoRuta.Servicios/Realtime/ViajeHubService.cs`.
- Create `MundoRuta.Servicios/Extensions/ServiceCollectionExtensions.cs`.
- Update `MundoRuta.Servicios.csproj`: add `MundoRuta.Shared`, `Microsoft.AspNetCore.SignalR.Client`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions`.
- Update `MundoRuta.Server.Client.csproj`: remove the direct `Microsoft.AspNetCore.SignalR.Client` reference.
- Update `MundoRuta.Server.Client/Program.cs`: register through the new extension method.
- Delete `MundoRuta.Server.Client/Servicios/ViajeHubService.cs` and `MundoRuta.Servicios/Class1.cs`.
- Do not touch `ViajeHub`, controllers, hub mapping, or Shared DTOs.

## Constraints

- No `NavigationManager` in `MundoRuta.Servicios`.
- Build the hub URI from `HttpClient.BaseAddress` plus the internal constant `hubs/viaje`.
- Preserve the four event names and mirror every group method exposed by `ViajeHub.cs`.
- Follow `.editorconfig`: block-scoped namespaces, usings outside the namespace, avoid `var`.
- Do not add UI or open a connection at startup.
- No test project exists; verification is build-based.

## Acceptance Criteria

- [x] `MundoRuta.Servicios` compiles on its own and has no Blazor references.
- [x] `MundoRuta.Server.Client` compiles without declaring `SignalR.Client`.
- [x] The service keeps all four events and covers every Hub group with join/leave methods.
- [x] `IniciarConexionAsync` is idempotent, thread-safe, and resets state after a failed start.
- [x] The old wrapper and `Class1.cs` are gone.
- [ ] `dotnet build MundoRuta.slnx` passes. Blocked by a pre-existing error in `MundoRuta.BD/Datos/Entity/Viaje.cs`: `CalificadoPorPasajero` is declared twice (lines 37 and 78) in `HEAD`. Both changed projects build successfully; the BD duplicate is outside this feature's scope.

## Tasks

- [x] TASK-01: Move the wrapper to `MundoRuta.Servicios`, add the contract, DI extension, dependencies, and clean up the Client. Route: delegated writer (`general`) — writer trigger: 6+ non-trivial files; mapping already done. Checks: `dotnet build MundoRuta.slnx`.
- [x] TASK-02: Verify and record evidence. Route: inline — per-action build check. Checks: build + grep for `NavigationManager` and the Client package reference.

## Checks

- [ ] `dotnet build MundoRuta.slnx` — fails with the pre-existing BD duplicate; not caused by this feature.
- [x] `dotnet build MundoRuta.Servicios/MundoRuta.Servicios.csproj` — 0 errors.
- [x] `dotnet build MundoRuta.Server/MundoRuta.Server.Client/MundoRuta.Server.Client.csproj` — 0 errors.
- [x] `rg "NavigationManager" MundoRuta.Servicios` returns nothing.
- [x] `rg "SignalR.Client" MundoRuta.Server/MundoRuta.Server.Client/MundoRuta.Server.Client.csproj` returns nothing.

## TDD

Not configured. The repository has no test project or runner, so checks are compilation-based.

## Delivery Strategy

`ask-on-risk`. Forecast: roughly 150 authored lines (additions plus deletions), below the ~400-line budget. No PR for now.

## Progress

- [x] TASK-01
- [x] TASK-02

## Evidence

### Files changed

- `MundoRuta.Servicios/Realtime/IViajeHubService.cs` — new contract with the four events and every Hub group join/leave method.
- `MundoRuta.Servicios/Realtime/ViajeHubService.cs` — new implementation; no `NavigationManager`, uses `HttpClient.BaseAddress`, `ILogger`, idempotent start, reconnect logging, full disposal.
- `MundoRuta.Servicios/Extensions/ServiceCollectionExtensions.cs` — `AddMundoRutaServicios(Uri)` registers `HttpClient` and `IViajeHubService`.
- `MundoRuta.Servicios/MundoRuta.Servicios.csproj` — added Shared, SignalR.Client, DI and logging abstraction references.
- `MundoRuta.Server/MundoRuta.Server.Client/MundoRuta.Server.Client.csproj` — removed the direct SignalR.Client package.
- `MundoRuta.Server/MundoRuta.Server.Client/Program.cs` — registers the service layer through the extension.
- Deleted `MundoRuta.Server/MundoRuta.Server.Client/Servicios/ViajeHubService.cs`.
- Deleted `MundoRuta.Servicios/Class1.cs`.

### Verification

- `dotnet build MundoRuta.Servicios/MundoRuta.Servicios.csproj` → success, 0 warnings, 0 errors.
- `dotnet build MundoRuta.Server/MundoRuta.Server.Client/MundoRuta.Server.Client.csproj` → success, 0 warnings, 0 errors.
- `dotnet build MundoRuta.slnx` → fails in `MundoRuta.BD/Datos/Entity/Viaje.cs(78,17)` with `CS0102: El tipo 'Viaje' ya contiene una definición para 'CalificadoPorPasajero'`.
- `dotnet build MundoRuta.BD/MundoRuta.BD.csproj` → the same CS0102 error; confirms the failure is pre-existing.
- `git show HEAD:MundoRuta.BD/Datos/Entity/Viaje.cs` → `CalificadoPorPasajero` is already declared at lines 37 and 78 before this feature.
- Grep: no `NavigationManager` or `MundoRuta.Server.Client` reference inside `MundoRuta.Servicios`.
- Grep: no direct `SignalR.Client` package in the Client csproj.

### Commit

This work-unit commit. The final hash is recorded in Engram and reported to the user.

### Review

Pending RDD assessment.

## Next Step

Commit this work unit and run the RDD assessment for the committed range. The full solution build remains blocked only by the pre-existing `Viaje` duplicate; do not expand scope to fix it without authorization.
