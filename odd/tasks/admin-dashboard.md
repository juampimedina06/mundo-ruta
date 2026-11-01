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

- [x] `dotnet build MundoRuta.slnx` passes.
- [x] `GET api/admin/dashboard` returns total users, prestadores, choferes, pending prestadores, total trips, active trips, volume, commissions and cut-off date.
- [ ] `/admin/dashboard` browser render not exercised; the page compiles and follows the `Pages/Public/Prestadores.razor` load pattern.
- [ ] Pending prestadores link present in markup; not visually verified.
- [ ] Date helper returns `dd/MM/yyyy`; not visually verified in the browser.
- [ ] Non-admin redirect not exercised in a browser.

## Tasks

- [x] TASK-01: Move ticket to `docs/tickets/admin-dashboard.md`, delete `MundoRuta.Shared/tickerEliminar.cs`. Route: inline — mechanical, restores the build.
- [x] TASK-02: Backend contract, repository aggregates, DI and controller. Route: delegated writer (`general`) — writer trigger: 6+ non-trivial files.
- [x] TASK-03: Service layer (`IAdminServicio`, `AdminServicio`, DI, `_Imports`). Route: delegated writer (`general`) — writer trigger: 4 non-trivial files.
- [x] TASK-04: Client page, KPI styles, date helper, menu and role route. Route: delegated writer (`general`) — writer trigger: 5 non-trivial files.
- [x] TASK-05: Build, endpoint smoke verification and record evidence. Route: inline — per-action build check.

## Checks

- [x] `dotnet build MundoRuta.slnx` — 0 errors (150 pre-existing warnings).
- [x] `GET http://localhost:5199/api/admin/dashboard` — 200 with `{"totalUsuarios":1,"totalPasajeros":1,"totalPrestadores":0,"totalChoferes":0,"prestadoresPendientes":0,"totalViajes":0,"viajesActivos":0,"volumenTransaccionado":0.00,"comisiones":0,"fechaCorte":"2026-11-01T20:25:54Z"}`.
- [x] EF logs confirmed the expected SQL: `COUNT(*)` over `Viajes`, `IN` over the five active states, `COALESCE(SUM(Monto))` for finalized trips, `COALESCE(SUM(Comision))` over `Liquidaciones`.
- [ ] Browser render of `/admin/dashboard` — pending manual smoke.

## TDD

Not configured. No test project or runner exists; checks are compilation-based plus an endpoint smoke test.

## Delivery Strategy

`single-pr`. Forecast: roughly 350 authored lines, below the 400-line budget. No PR and no push unless the user asks.

## Progress

- [x] TASK-01
- [x] TASK-02
- [x] TASK-03
- [x] TASK-04
- [x] TASK-05

## Evidence

### Files changed

- `docs/tickets/admin-dashboard.md` — ticket text preserved as documentation.
- Deleted `MundoRuta.Shared/tickerEliminar.cs` — it was invalid C# and broke the solution build with 30 errors.
- `MundoRuta.Shared/DTO/AdminDashboardDTO.cs` — dashboard contract with the ten KPI fields.
- `MundoRuta.Repositorio/Repositorios/IViajeRepositorio.cs` — `ContarTodos`, `ContarActivos`, `SumarMontoTransaccionado`.
- `MundoRuta.Repositorio/Repositorios/ViajeRepositorio.cs` — implementations; active states built from `EstadosViaje` constants.
- `MundoRuta.Repositorio/Repositorios/ILiquidacionRepositorio.cs` — `SumarComisiones`.
- `MundoRuta.Repositorio/Repositorios/LIquidacionRespositorio.cs` — implementation with empty-table coalesce.
- `MundoRuta.Server/MundoRuta.Server/Controllers/AdminController.cs` — `GetDashboard` returns `AdminDashboardDTO`.
- `MundoRuta.Server/MundoRuta.Server/Program.cs` — registers `ILiquidacionRepositorio` (was missing from DI).
- `MundoRuta.Servicios/Admin/IAdminServicio.cs` and `AdminServicio.cs` — client contract and implementation of `GET api/admin/dashboard`.
- `MundoRuta.Servicios/Extensions/ServiceCollectionExtensions.cs` — registers `IAdminServicio`.
- `MundoRuta.Server/MundoRuta.Server.Client/_Imports.razor` — global imports for `MundoRuta.Shared.DTO` and `MundoRuta.Servicios.Admin`.
- `MundoRuta.Server/MundoRuta.Server.Client/Pages/Admin/Dashboard.razor` — `/admin/dashboard` with `RoleGuard`, loading/error/empty states and four KPI cards.
- `MundoRuta.Server/MundoRuta.Server.Client/Infraestructura/FormatoPanel.cs` — `dd/MM/yyyy` with `ToLocalTime()` and `$ ` money formatting.
- `MundoRuta.Server/MundoRuta.Server/wwwroot/app.css` — `.mr-kpi*` styles.
- `MundoRuta.Server/MundoRuta.Server.Client/Navigation/MenuPorRol.cs` — Dashboard first in the admin menu.
- `MundoRuta.Server/MundoRuta.Server.Client/Navigation/RutaInicioPorRol.cs` — admin home route now `/admin/dashboard`.

### Commits

- `8a7c1b6` chore(docs): mover ticket de dashboard admin a docs/tickets
- `fa16ad4` feat(api): exponer KPIs completos en el dashboard de admin
- `bd27702` feat(servicios): agregar IAdminServicio para el dashboard
- `c0c8aeb` feat(client): crear pagina de dashboard del administrador

### Notes

- `RoleGuard Permitidos="Roles.Admin"` binds the literal string, not the constant. The new page uses `Permitidos="@Roles.Admin"`. `Pages/Admin/Panel.razor` has the same pre-existing bug (admin is denied on `/admin`); left untouched because it is outside this feature's scope.
- `Pagos` is never written by the application, so transacted volume is derived from finalized `Viaje.Monto`.

### Review

Pending RDD assessment.

## Next Step

Manual browser smoke of `/admin/dashboard` with the seeded admin, then decide whether to open a PR (not pushed).
