# Feature: Admin — user directory

## Objective

Build the centralized directory of every registered user for support, audit and profile lookup, at Blazor route `/admin/usuarios`, backed by two endpoints:

- `GET api/admin/usuarios`
- `GET api/admin/usuarios/{id}`

The admin must be able to search by name or email, filter by role and account state, and open a detail card for any row without leaving the page.

## Problem

Neither endpoint exists. `AdminController` only exposes `prestadores-pendientes`, `prestadores/solicitudes`, `prestadores/{id}/estado`, `dashboard` and the acompanante-configuration pair. There is no user-list DTO, no repository projection over `Usuarios`, no aggregate per user (trips, ratings), no `IAdminServicio` method, no `/admin/usuarios` page, and no modal component in the client.

Two structural gaps shape the work:

1. `IRepositorio<Usuario>.Select()` returns full `Usuario` entities **including `Password`**, so the admin list must use an explicit projection.
2. `Calificacion` stores `IdUsuario` (who rates) and `IdUsuarioPrestador` (who is rated). Only the rated side has historical data, so a plain `Usuario` passenger has no received average. The ticket asks for the average on every profile, so the detail contract exposes both directions: received and given.

Also, the ticket arrived as `MundoRuta.Shared/ticketEliminar.cs` — plain text in a C# file — and breaks the build with 33 errors (CS1002, CS1022, CS1525).

## Why

The ticket in `MundoRuta.Shared/ticketEliminar.cs` is the spec. The user selected, in the planning round, the trip-count semantics per role and asked that the `Usuario` role also carry a rating figure. Both decisions are recorded in Scope below.

## Scope

- Move the ticket text to `docs/tickets/admin-usuarios.md`; delete the invalid `.cs`.
- New `AdminUsuarioListadoDTO` (row) and `AdminUsuarioDetalleDTO` (detail card) in `MundoRuta.Shared`.
- `IUsuarioRepositorio.ListarUsuarios()` — explicit projection, no `Password`, UTC-stamped dates.
- `IViajeRepositorio.ContarViajesDeUsuario(int)` and `ContarViajesDeChofer(int)`.
- `ICalificacionRepositorio.ObtenerPromedioRecibido(int)` and `ObtenerPromedioOtorgado(int)`, each returning average and total.
- `IChoferRepositorio.ObtenerPorUsuario(int)` to resolve the `Chofer` row behind a `Chofer` user.
- Two `AdminController` endpoints; detail composition in the controller, not the repository.
- `IAdminServicio` / `AdminServicio`: list and detail.
- `Pages/Admin/Usuarios.razor` with search, role chips, state select, table and detail modal.
- `Components/DetalleUsuarioModal.razor` — JS-free overlay driven by component state.
- Admin menu entry, `Panel.razor` card, `RoleGuard` fixes, `app.css` modal and badge styles.

### Trip-count semantics (decided)

- `Usuario`, `Prestador`, `Admin`: trips where the user appears as `IdUsuario`, `IdPasajero` or `IdSolicitante`.
- `Chofer`: `Viaje.IdChofer` references `Chofer.Id`, not `Usuario.Id`, so the controller resolves the `Chofer` row first and counts by that id.

### Rating semantics (decided)

- `PromedioRecibido` / `TotalRecibidas`: `Calificacion.IdUsuarioPrestador == id`.
- `PromedioOtorgado` / `TotalOtorgadas`: `Calificacion.IdUsuario == id`.
- Both are computed for every role. `null` when there are no rows, so the UI can render `—` instead of a fake zero.

## Constraints

- Work on the existing branch `feature/admin-gestionUsuarios`. No push, no PR.
- Follow `.editorconfig`: block-scoped namespaces, usings outside the namespace, avoid `var`, nullable enabled, 4 spaces, CRLF.
- `MundoRuta.Servicios` references only `Shared` — never `BD` or `Repositorio`.
- Never return `Usuario.Password`. Never call `IRepositorio<Usuario>.Select()` for the list.
- SQL Server returns `DateTime` with `Kind = Unspecified`; stamp `DateTimeKind.Utc` before returning so `FormatoPanel.Fecha` (`ToLocalTime()`) is correct.
- `GET api/admin/usuarios` always returns 200 with an array, never 404.
- `GET api/admin/usuarios/{id}` returns 404 when the user does not exist.
- Filtering is client-side over one loaded list: one request, no per-keystroke round trips.
- No auth exists in the project; this feature does not add `[Authorize]`. Existing IDOR risk is untouched tech debt.
- Modal is a JS-free conditional overlay. Bootstrap JS is not loaded and is not added.
- Reuse `EstadoBadge`, `EmptyState`, `PageHeader`, `RoleGuard`, `FormatoPanel.Fecha`.
- No test project or runner exists; verification is build plus endpoint and browser smoke.
- Do not modify `UsuarioDTO` (no `Id`, consumed by existing screens) or `PrestadorListadoDTO` (public listing, CUIT leakage risk).

## Acceptance Criteria

- [ ] `dotnet build MundoRuta.slnx` passes with 0 errors.
- [ ] `GET api/admin/usuarios` returns every user ordered by `FechaRegistro` desc with Id, Nombre, Apellido, Email, Rol, Estado and FechaRegistro, and never exposes Password.
- [ ] `GET api/admin/usuarios/{id}` returns contact data, both rating aggregates and the trip count; 404 for an unknown id.
- [ ] For a `Prestador`, the detail includes vehicles and drivers (flota asociada).
- [ ] `/admin/usuarios` renders the table under `RoleGuard` for admin only.
- [ ] Search by name or email and filters by role and state narrow the table without network traffic.
- [ ] "Ver detalle" opens the modal with the full profile and closes without a page reload.

## Tasks

- [ ] TASK-01: Create this feature document and the Engram mirror. Route: inline — mechanical, no research left.
- [ ] TASK-02: Move the ticket to `docs/tickets/admin-usuarios.md`, delete `MundoRuta.Shared/ticketEliminar.cs`. Route: delegated writer — first write after planning, bundled with the backend batch it unblocks.
- [ ] TASK-03: Shared contracts (`AdminUsuarioListadoDTO`, `AdminUsuarioDetalleDTO`). Route: delegated writer.
- [ ] TASK-04: Repository queries (users projection, trip counts, rating aggregates, chofer-by-user). Route: delegated writer — same batch, all depend on TASK-03's contract.
- [ ] TASK-05: `AdminController` endpoints and detail composition. Route: delegated writer — same batch.
- [ ] TASK-06: `IAdminServicio` / `AdminServicio`. Route: delegated writer — same batch.
- [ ] TASK-07: `/admin/usuarios` page and `DetalleUsuarioModal` component. Route: delegated writer — the largest file set, contract already fixed by TASK-03.
- [ ] TASK-08: Admin menu entry, `Panel.razor` card, `RoleGuard` fixes, `app.css` styles. Route: delegated writer — same frontend batch.
- [ ] TASK-09: Build, endpoint smoke and browser smoke. Route: inline — per-action build and HTTP checks.

## Checks

- [ ] `dotnet build MundoRuta.slnx` — 0 errors.
- [ ] `GET api/admin/usuarios` — 200 with the seeded admin and the registered test users.
- [ ] `GET api/admin/usuarios/{id}` for each of the four roles — 200 with role-appropriate detail.
- [ ] `GET api/admin/usuarios/999999` — 404.
- [ ] Response payload scan — no `password` field anywhere.
- [ ] `/admin/usuarios` browser smoke — table, search, both filters, modal open and close.

## TDD

Not configured. No test project, no runner. Checks are compilation-based plus endpoint and manual smoke.

## Delivery Strategy

`ask-on-risk`. Forecast from the task list: ~620 authored changed lines, above the 400-line review budget. Two natural slices are recorded: backend (TASK-02..TASK-06) and client (TASK-07..TASK-08). No push and no PR have been made; the delivery and chain decisions stay with the user.

## Progress

- [ ] TASK-01
- [ ] TASK-02
- [ ] TASK-03
- [ ] TASK-04
- [ ] TASK-05
- [ ] TASK-06
- [ ] TASK-07
- [ ] TASK-08
- [ ] TASK-09

## Evidence

### Files changed

Added:

- `docs/tickets/admin-usuarios.md` — preserved ticket text.
- `odd/tasks/admin-usuarios.md` — this document.

Modified:

- (recorded as tasks close)

Deleted:

- `MundoRuta.Shared/ticketEliminar.cs` — plain text in a `.cs` file, broke the build.

### Commits

- (recorded as tasks close)

### Notes

(pending)

### Review

(pending)

## Next Step

TASK-02: unblock the build, then the backend batch.
