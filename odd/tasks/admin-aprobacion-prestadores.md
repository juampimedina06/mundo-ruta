# Feature: Admin — prestador request inbox

## Objective

Embed the prestador (transport company) onboarding inbox inside `/admin/dashboard`, below the KPI grid, and back it with the two endpoints the ticket defines:

- `GET api/admin/prestadores/solicitudes`
- `PUT api/admin/prestadores/{id}/estado`

The admin must be able to search, filter by state, and approve, reject or suspend a company without reloading the page.

## Problem

The ticket asks for a working approval inbox, but neither endpoint exists. Today the backend only offers `GET api/admin/prestadores-pendientes` (PENDIENTE only, returns 404 when empty, and its `PrestadorListadoDTO` carries no RazonSocial/Cuit/FechaRegistro) and `PUT api/admin/aprobar-prestador/{id}` (forces APROBADO, validates nothing). `Pages/Admin/Prestadores.razor` is an empty placeholder and `IAdminServicio` only exposes the dashboard KPIs, so the client cannot render the inbox.

A second, non-cosmetic gap: `AuthController.Login` blocks only `PENDIENTE`. A company marked `RECHAZADO` or `SUSPENDIDO` keeps logging in and operating, which contradicts the ticket's reject/suspend actions.

Also, the ticket file arrived as `MundoRuta.Shared/ticketEliminar.cs` — plain text in a C# file — and breaks the build.

## Why

The ticket in `MundoRuta.Shared/ticketEliminar.cs` is the spec. The user explicitly overrode the Blazor route: the inbox lives inside the dashboard, below the metrics, not on its own page. The API routes stay as the ticket defines them.

## Scope

- Move the ticket text to `docs/tickets/admin-aprobacion-prestadores.md`; delete the invalid `.cs`.
- New `PrestadorSolicitudDTO` (admin-only fields) and `ActualizarEstadoPrestadorDTO`.
- `IUsuarioRepositorio.ListarSolicitudesPrestador()`.
- Two `AdminController` endpoints with state and role validation.
- `AuthController` login guard for RECHAZADO and SUSPENDIDO.
- `IAdminServicio` / `AdminServicio`: list requests, update state.
- `Pages/Admin/Dashboard.razor`: inbox section below the KPIs, search, state chips, inline confirmation, in-place row update plus KPI refresh.
- `app.css`: table, toolbar, chip and badge-variant styles.
- `Pages/Admin/Prestadores.razor` becomes a redirect to the dashboard anchor; admin menu entry removed.

## Constraints

- Work on `feature/admin-aprobacionPrestador`. No push, no PR unless asked.
- Follow `.editorconfig`: block-scoped namespaces, usings outside namespace, avoid `var`, nullable enabled.
- `MundoRuta.Servicios` must reference only `Shared` — never `BD` or `Repositorio`.
- Do not add RazonSocial/Cuit to `PrestadorListadoDTO`: it feeds the public `GET api/servicios/fletes` listing and would leak CUIT publicly.
- Allowed provider states: `APROBADO`, `RECHAZADO`, `SUSPENDIDO`. Reject anything else with 400.
- `GET .../solicitudes` returns 200 with an empty array, never 404 — the UI must distinguish empty from error.
- Filtering is client-side over one loaded list: one request, instant feedback, no per-keystroke round trips.
- Confirmation is inline state in the component; no `IJSRuntime` `confirm()`.
- Reuse `EstadoBadge`, `EmptyState`, `FormatoPanel.Fecha` (`dd/MM/yyyy`, `ToLocalTime()`).
- No test project or runner exists; verification is build-based plus endpoint and browser smoke.
- Legacy `PUT api/admin/aprobar-prestador/{id}` stays untouched (no consumer); removing it is separate tech debt.

## Acceptance Criteria

- [x] `dotnet build MundoRuta.slnx` passes.
- [x] `GET api/admin/prestadores/solicitudes` returns every `Prestador` ordered by registration date desc, with RazonSocial, NombreCompleto, Cuit, Email, Telefono, FechaRegistro and Estado.
- [x] `PUT api/admin/prestadores/{id}/estado` accepts APROBADO / RECHAZADO / SUSPENDIDO, rejects other values with 400, and rejects non-prestador ids with 404.
- [x] Login returns 403 for PENDIENTE, RECHAZADO and SUSPENDIDO; 200 after APROBADO.
- [x] `/admin/dashboard` renders the inbox below the KPI cards.
- [x] Search and state chips filter the table without a server round trip.
- [x] Approve/reject/suspend updates the row in place, shows feedback and refreshes the "Prestadores pendientes" KPI.

## Tasks

- [x] TASK-01: Move the ticket to `docs/tickets/admin-aprobacion-prestadores.md`, delete `MundoRuta.Shared/ticketEliminar.cs`, create this document. Route: inline — mechanical, restores the build.
- [x] TASK-02: Backend contract, repository query, both admin endpoints and the login guard. Route: inline. The delegated `general` writer was cancelled: the remaining work was a known edit set with no open design questions, and the parent already held the exact contract, so delegating added a handoff without removing research. Deviation noted rather than hidden.
- [x] TASK-03: Service layer (`IAdminServicio`, `AdminServicio`). Route: inline — two arithmetical additions on top of TASK-02's contract.
- [x] TASK-04: Dashboard inbox section, styles, redirect page and admin menu. Route: inline — the layout depended on decisions already made in the plan (filters, inline confirm, anchor), so a writer would have re-derived them.
- [x] TASK-05: Build, endpoint smoke and browser smoke evidence. Route: inline — per-action build and HTTP checks.

## Checks

- [x] `dotnet build MundoRuta.slnx` — 0 errors.
- [x] `POST api/auth/register` with `Rol=Prestador` then `GET api/admin/prestadores/solicitudes` — new company present.
- [x] `PUT api/admin/prestadores/{id}/estado` APROBADO — 200 and login 200.
- [x] `PUT api/admin/prestadores/{id}/estado` RECHAZADO — 200 and login 403.
- [x] `PUT api/admin/prestadores/{id}/estado` with `INVALIDO` — 400.
- [x] `PUT api/admin/prestadores/{id}/estado` over a `Usuario` id — 404.
- [ ] Browser smoke of `/admin/dashboard` — not exercised. The Blazor UI was verified by compilation only; the section, search, chips and inline confirmation still need a manual pass in the browser.

## TDD

Not configured. No test project or runner exists; checks are compilation-based plus endpoint and manual smoke.

## Delivery Strategy

`single-pr`. Actual volume: 513 insertions, 24 deletions in code — roughly 537 authored changed lines, above the 400-line review budget. A single PR for this feature therefore needs `size:exception`, or a split into slices (backend contract/API, then service layer + client). No PR and no push have been made; that decision stays with the user.

## Progress

- [x] TASK-01
- [x] TASK-02
- [x] TASK-03
- [x] TASK-04
- [x] TASK-05 (endpoint smoke green; browser smoke pending)

## Evidence

### Files changed

Added:

- `MundoRuta.Shared/EstadosPrestador.cs` — `Pendiente`, `Aprobado`, `Rechazado`, `Suspendido`, private `EstadosAsignables`, `EsAsignable(string?)`.
- `MundoRuta.Shared/DTO/PrestadorSolicitudDTO.cs` — admin-only view: `Id`, `RazonSocial`, `NombreCompleto`, `Cuit`, `Email`, `Telefono`, `FechaRegistro`, `Estado`.
- `MundoRuta.Shared/DTO/ActualizarEstadoPrestadorDTO.cs` — `Estado`.
- `docs/tickets/admin-aprobacion-prestadores.md` — preserved ticket text.

Modified:

- `MundoRuta.Repositorio/Repositorios/IUsuarioRepositorio.cs`, `UsuarioRepositorio.cs` — `ListarSolicitudesPrestador()`: filters `Rol == Roles.Prestador`, orders by `FechaRegistro` desc, and stamps `DateTimeKind.Utc` on the materialized dates.
- `MundoRuta.Server/MundoRuta.Server/Controllers/AdminController.cs` — `GET prestadores/solicitudes` (always 200, `[]` when empty) and `PUT prestadores/{id}/estado` (400 on invalid state, 404 when the id is missing or is not a `Prestador`).
- `MundoRuta.Server/MundoRuta.Server/Controllers/AuthController.cs` — `PENDIENTE` guard now uses `EstadosPrestador.Pendiente`; new 403 for a `Prestador` that is `RECHAZADO` or `SUSPENDIDO`.
- `MundoRuta.Servicios/Admin/IAdminServicio.cs`, `AdminServicio.cs` — `ObtenerSolicitudesPrestadoresAsync()`, `ActualizarEstadoPrestadorAsync(int, string)`.
- `MundoRuta.Server/MundoRuta.Server.Client/Pages/Admin/Dashboard.razor` — inbox section below the KPIs with search, state chips, table, inline confirmation, feedback banners and KPI reload after a change.
- `MundoRuta.Server/MundoRuta.Server.Client/Pages/Admin/Prestadores.razor` — redirect to `/admin/dashboard#solicitudes-prestadores`.
- `MundoRuta.Server/MundoRuta.Server.Client/Navigation/MenuPorRol.cs` — removed the admin `Prestadores` entry.
- `MundoRuta.Server/MundoRuta.Server/wwwroot/app.css` — section, toolbar, search, chips, table, actions and badge-variant styles.

Deleted:

- `MundoRuta.Shared/ticketEliminar.cs` — plain text in a `.cs` file, broke the build with CS1002/CS1022/CS1525.

### Commits

- `a9072c2` — `docs(odd): planificar bandeja de prestadores del dashboard admin`
- `8934e04` — `feat(shared): agregar estados de prestador y DTOs de la bandeja admin`
- `f440b4e` — `feat(repositorio): listar solicitudes de prestadores con fecha en UTC`
- `58b9e3b` — `feat(api): exponer bandeja y cambio de estado de prestadores`
- `0eb1340` — `feat(servicios): consultar y actualizar estado de prestadores`
- `295254f` — `feat(client): integrar bandeja de prestadores en el dashboard admin`

### Notes

Endpoint smoke against `http://localhost:5090` (LocalDB, Development). The test company was registered, exercised and deleted afterwards; the database is left as it was found.

```
01 GET  /api/admin/prestadores/solicitudes                  -> 200 []
02 POST /api/auth/register (Prestador)                      -> 200 id=3
03 GET  /api/admin/prestadores/solicitudes                  -> 200 [ ...estado PENDIENTE... ]
04 PUT  estado=INVALIDO                                     -> 400
05 PUT  id inexistente                                      -> 404
06 POST /api/auth/login (PENDIENTE)                          -> 403
07 PUT  estado=APROBADO                                     -> 200
08 POST /api/auth/login (APROBADO)                           -> 200
09 PUT  estado=RECHAZADO                                    -> 200
10 POST /api/auth/login (RECHAZADO)                          -> 403
11 PUT  estado=SUSPENDIDO                                   -> 200
12 POST /api/auth/login (SUSPENDIDO)                         -> 403
13 PUT  sobre usuario Admin id=1                             -> 404
14 limpieza: filas de prueba borradas = 1
15 GET  /api/admin/prestadores/solicitudes                   -> 200 []
```

`dotnet build MundoRuta.slnx` — 0 errors, 150 warnings (the pre-existing baseline).

One defect found and fixed during the smoke: SQL Server returns `FechaRegistro` with `Kind = Unspecified`, so `FormatoPanel.Fecha`'s `ToLocalTime()` was a no-op and the ticket date would have been shown as the raw UTC date (off by one day for requests between 21:00 and 23:59 in UTC-3). The repository now materializes and stamps `DateTimeKind.Utc`; the API re-verified as `2026-11-02T01:27:53.9208332Z`.

### Review

Receipt-driven development was explicitly declined by the user (`no tengo go pago`); no `gentle-ai review` lifecycle was run on this feature. Verification was the build plus the endpoint smoke recorded above, and delivery follows ordinary repository policy.

## Next Step

Manual browser pass of `/admin/dashboard`: confirm the inbox sits below the KPI grid, that search and the state chips filter without network traffic, that the inline confirmation updates the row and the "Prestadores pendientes" KPI, and that `Aprobar/Rechazar/Suspender` are disabled for the row's current state. Then decide the delivery shape: one PR with `size:exception` (~537 authored lines) or a split into backend and client slices.
