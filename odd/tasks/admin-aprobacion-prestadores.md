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

- [ ] `dotnet build MundoRuta.slnx` passes.
- [ ] `GET api/admin/prestadores/solicitudes` returns every `Prestador` ordered by registration date desc, with RazonSocial, NombreCompleto, Cuit, Email, Telefono, FechaRegistro and Estado.
- [ ] `PUT api/admin/prestadores/{id}/estado` accepts APROBADO / RECHAZADO / SUSPENDIDO, rejects other values with 400, and rejects non-prestador ids with 404.
- [ ] Login returns 403 for PENDIENTE, RECHAZADO and SUSPENDIDO; 200 after APROBADO.
- [ ] `/admin/dashboard` renders the inbox below the KPI cards.
- [ ] Search and state chips filter the table without a server round trip.
- [ ] Approve/reject/suspend updates the row in place, shows feedback and refreshes the "Prestadores pendientes" KPI.

## Tasks

- [ ] TASK-01: Move the ticket to `docs/tickets/admin-aprobacion-prestadores.md`, delete `MundoRuta.Shared/ticketEliminar.cs`, create this document. Route: inline — mechanical, restores the build.
- [ ] TASK-02: Backend contract, repository query, both admin endpoints and the login guard. Route: delegated writer (`general`) — writer trigger: 6 non-trivial files.
- [ ] TASK-03: Service layer (`IAdminServicio`, `AdminServicio`). Route: same delegated writer — one arithmetic dependency on TASK-02's DTOs.
- [ ] TASK-04: Dashboard inbox section, styles, redirect page and admin menu. Route: same delegated writer — writer trigger: 4 non-trivial files.
- [ ] TASK-05: Build, endpoint smoke and browser smoke evidence. Route: inline — per-action build and HTTP checks.

## Checks

- [ ] `dotnet build MundoRuta.slnx` — 0 errors.
- [ ] `POST api/auth/register` with `Rol=Prestador` then `GET api/admin/prestadores/solicitudes` — new company present.
- [ ] `PUT api/admin/prestadores/{id}/estado` APROBADO — 200 and login 200.
- [ ] `PUT api/admin/prestadores/{id}/estado` RECHAZADO — 200 and login 403.
- [ ] `PUT api/admin/prestadores/{id}/estado` with `INVALIDO` — 400.
- [ ] `PUT api/admin/prestadores/{id}/estado` over a `Usuario` id — 404.
- [ ] Browser smoke of `/admin/dashboard` — not exercised.

## TDD

Not configured. No test project or runner exists; checks are compilation-based plus endpoint and manual smoke.

## Delivery Strategy

`single-pr`. Forecast: roughly 300 authored lines, below the 400-line budget. No PR and no push unless the user asks.

## Progress

- [ ] TASK-01
- [ ] TASK-02
- [ ] TASK-03
- [ ] TASK-04
- [ ] TASK-05

## Evidence

### Files changed

Pending.

### Commits

Pending.

### Notes

Pending.

### Review

Pending RDD assessment.

## Next Step

Pending.
