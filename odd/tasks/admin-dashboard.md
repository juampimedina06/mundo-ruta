# Feature: Admin Dashboard Panel

## Objective

Build the administrator control panel at `/admin/dashboard` that consumes `GET api/admin/dashboard` through a new `IAdminServicio` in `MundoRuta.Servicios` and renders the platform KPIs.

## Problem

`AdminController.GetDashboard` returns an anonymous object with three fields (`totalUsuarios`, `totalPrestadores`, `viajesConcretos`). There is no DTO contract, no `IAdminServicio`, and `Pages/Admin/Panel.razor` shows no data. The ticket also requires KPIs that do not exist today: pending prestadores, total and active trips across five states, transacted volume, commissions, and `dd/mm/aaaa` date formatting.

## Why

The ticket in `MundoRuta.Shared/tickerEliminar.cs` (invalid C#, treated as the spec) asks for the admin dashboard. The service layer already exists (`IHttpServices`, `AuthServicio` as pattern), so the work is contract + repo aggregates + endpoint + service + page.

## Scope

- Move the ticket text to `docs/tickets/admin-dashboard.md` and delete the invalid `.cs` that breaks the build.
- New `MundoRuta.Shared/DTO/AdminDashboardDTO.cs`.
- Aggregates: `IViajeRepositorio` (total, active by the five states, sum of finalized amounts) and `ILiquidacionRepositorio` (sum of commissions); register liquidacion repo in server DI.
- Rewrite `AdminController.GetDashboard` to return the DTO.
- New `MundoRuta.Servicios/Admin/IAdminServicio.cs` and `AdminServicio.cs`; register in `ServiceCollectionExtensions`.
- New `Pages/Admin/Dashboard.razor`, KPI styles, date helper, menu entry and role home route.

## Constraints

- Do not push and do not switch branches. Work on `feature/admin-dashboard`.
- Follow `.editorconfig`: block-scoped namespaces, usings outside namespace, avoid `var`.
- `MundoRuta.Servicios` must not reference `BD` or `Repositorio`; only `Shared`.
- "Total Pasajeros" = users with `Rol = "Usuario"` (user decision).
- Active trips use `MundoRuta.Shared/EstadosViaje.cs` constants, never string literals.
- Transacted volume = `SUM(Viaje.Monto)` for `FINALIZADO` (the `Pagos` table is never written).
- Commissions = `SUM(Liquidacion.Comision)`.
- Money entities use `int`; the DTO exposes `decimal`.
- Dates are stored as UTC; format with `ToLocalTime()` and `dd/MM/aaaa`.
- No test project exists; verification is build-based plus manual smoke.

## Acceptance Criteria

- [ ] `dotnet build MundoRuta.slnx` passes.
- [ ] `GET api/admin/dashboard` returns total users, prestadores, choferes, pending prestadores, total trips, active trips, volume, commissions and cut-off date.
- [ ] `/admin/dashboard` loads the data through `IAdminServicio`, with loading, error and empty states.
- [ ] Pending prestadores card links to `/admin/prestadores`.
- [ ] Dates render as `dd/mm/aaaa`.
- [ ] Non-admin users are redirected by `RoleGuard`.

## Tasks

- [x] TASK-01: Move ticket to `docs/tickets/admin-dashboard.md`, delete `MundoRuta.Shared/tickerEliminar.cs`. Route: inline — mechanical, restores the build.
- [ ] TASK-02: Backend contract, repository aggregates, DI and controller. Route: delegated writer (`general`) — writer trigger: 6+ non-trivial files.
- [ ] TASK-03: Service layer (`IAdminServicio`, `AdminServicio`, DI, `_Imports`). Route: delegated writer (`general`) — writer trigger: 4 non-trivial files.
- [ ] TASK-04: Client page, KPI styles, date helper, menu and role route. Route: delegated writer (`general`) — writer trigger: 5 non-trivial files.
- [ ] TASK-05: Build, smoke verification and record evidence. Route: inline — per-action build check.

## Checks

- [ ] `dotnet build MundoRuta.slnx` — 0 errors.
- [ ] Manual smoke of `/admin/dashboard` with the seeded admin.

## TDD

Not configured. No test project or runner exists; checks are compilation-based plus manual smoke.

## Delivery Strategy

`single-pr`. Forecast: roughly 350 authored lines, below the 400-line budget. No PR and no push unless the user asks.

## Progress

- [x] TASK-01
- [ ] TASK-02
- [ ] TASK-03
- [ ] TASK-04
- [ ] TASK-05

## Evidence

Pending.

## Next Step

Start TASK-01.
