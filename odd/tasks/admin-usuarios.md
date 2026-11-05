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

- [x] `dotnet build MundoRuta.slnx` passes with 0 errors.
- [x] `GET api/admin/usuarios` returns every user ordered by `FechaRegistro` desc with Id, Nombre, Apellido, Email, Rol, Estado and FechaRegistro, and never exposes Password.
- [x] `GET api/admin/usuarios/{id}` returns contact data, both rating aggregates and the trip count; 404 for an unknown id.
- [x] For a `Prestador`, the detail includes vehicles and drivers (flota asociada).
- [x] `/admin/usuarios` renders the table under `RoleGuard` for admin only.
- [x] Search by name or email and filters by role and state narrow the table without network traffic.
- [x] "Ver detalle" opens the modal with the full profile and closes without a page reload.

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

- [x] `dotnet build MundoRuta.slnx` — 0 errors, 150 warnings (pre-existing baseline).
- [x] `GET api/admin/usuarios` — 200 with 3 users, ordered by `FechaRegistro` desc.
- [x] `GET api/admin/usuarios/{id}` for `Admin`, `Prestador` and `Usuario` — 200 with role-appropriate detail.
- [ ] `GET api/admin/usuarios/{id}` for a `Chofer` user — not exercised, no `Chofer` user exists in the local database. The branch is covered only by compilation.
- [x] `GET api/admin/usuarios/999999` — 404.
- [x] Response payload scan — no `password` field in either endpoint.
- [x] `/admin/usuarios` browser smoke — exercised with Playwright and the system Edge: table renders, search by name and email narrows correctly, role chip and state select work with one single list request, the three detail modals open with data and close. The smoke found and confirmed the fix for the `_errorDetalle` literal binding (see Post-smoke bugfix).

## TDD

Not configured. No test project, no runner. Checks are compilation-based plus endpoint and manual smoke.

## Delivery Strategy

`ask-on-risk`. Actual volume: 855 insertions, 3 deletions in code (`*.cs`, `*.razor`, `*.css`) — 858 authored changed lines, above the 400-line review budget. The two recorded slices are backend (TASK-02..TASK-06, 232 lines) and client (TASK-07..TASK-08, 626 lines). No push and no PR have been made; the delivery and chain decisions stay with the user.

## Progress

- [x] TASK-01
- [x] TASK-02
- [x] TASK-03
- [x] TASK-04
- [x] TASK-05
- [x] TASK-06
- [x] TASK-07
- [x] TASK-08
- [x] TASK-09 (endpoint and browser smoke green)

## Evidence

### Files changed

Added:

- `docs/tickets/admin-usuarios.md` — preserved ticket text.
- `odd/tasks/admin-usuarios.md` — this document.
- `MundoRuta.Shared/DTO/AdminUsuarioListadoDTO.cs` — row contract.
- `MundoRuta.Shared/DTO/AdminUsuarioDetalleDTO.cs` — detail contract with both rating aggregates.

Modified:

- `MundoRuta.Repositorio/Repositorios/IUsuarioRepositorio.cs`, `UsuarioRepositorio.cs` — `ListarUsuarios()`: explicit projection (no Password), `AsNoTracking`, ordered by `FechaRegistro` desc, UTC-stamped.
- `MundoRuta.Repositorio/Repositorios/IViajeRepositorio.cs`, `ViajeRepositorio.cs` — `ContarViajesDeUsuario(int)`, `ContarViajesDeChofer(int)`.
- `MundoRuta.Repositorio/Repositorios/ICalificacionRepositorio.cs`, `CalificacionRepositorio.cs` — `ObtenerPromedioRecibido(int)`, `ObtenerPromedioOtorgado(int)`, both returning `(double? Promedio, int Total)`.
- `MundoRuta.Repositorio/Repositorios/IChoferRepositorio.cs`, `ChoferRepositorio.cs` — `ObtenerPorUsuario(int)`.
- `MundoRuta.Server/MundoRuta.Server/Controllers/AdminController.cs` — `GET usuarios`, `GET usuarios/{id:int}`, role-aware detail composition.
- `MundoRuta.Servicios/Admin/IAdminServicio.cs`, `AdminServicio.cs` — `ObtenerUsuariosAsync()`, `ObtenerUsuarioAsync(int)`.
- `MundoRuta.Server/MundoRuta.Server.Client/Pages/Admin/Usuarios.razor` — directory page: loading/error/empty, search by name or email, role chips, state select, table, row action.
- `MundoRuta.Server/MundoRuta.Server.Client/Components/DetalleUsuarioModal.razor` — JS-free overlay with contact grid, both rating aggregates, trip count and, for `Prestador`, fleet and drivers.
- `MundoRuta.Server/MundoRuta.Server/wwwroot/app.css` — `.mr-modal*`, `.mr-detalle__*`, `.mr-filtro` and role badge variants (`--usuario`, `--chofer`, `--admin`, `--inactivo`).
- `MundoRuta.Server/MundoRuta.Server.Client/Navigation/MenuPorRol.cs` — admin `Usuarios` entry.
- `MundoRuta.Server/MundoRuta.Server.Client/Pages/Admin/Panel.razor` — third card linking to `/admin/usuarios`, plus `RoleGuard` fix.
- `MundoRuta.Server/MundoRuta.Server.Client/Pages/Admin/Configuracion.razor` — `RoleGuard` fix.

Deleted:

- `MundoRuta.Shared/ticketEliminar.cs` — plain text in a `.cs` file, broke the build with 33 errors. It was untracked, so the deletion does not appear in any commit diff.

### Commits

- `26923b4` — `docs(odd): planificar el directorio de usuarios del admin`
- `813bb5d` — `chore: mover el ticket de usuarios a docs y desbloquear el build`
- `ba5dfb5` — `feat(shared): agregar DTOs del directorio de usuarios admin`
- `98ee290` — `feat(repositorio): consultar usuarios, viajes y calificaciones para el admin`
- `12a6efa` — `feat(api): exponer el directorio de usuarios admin`
- `02983d9` — `feat(servicios): consultar usuarios desde el cliente`
- `aecdbb7` — `feat(client): crear el directorio de usuarios del admin`
- `9d8d090` — `feat(client): enlazar usuarios en el panel admin`

### Notes

Endpoint smoke against `http://localhost:5099` (LocalDB, Development). No test data was created; the database was only read.

```
01 GET /api/admin/usuarios            -> 200, 3 usuarios, orden fechaRegistro DESC
   campos: id, nombre, apellido, email, rol, estado, fechaRegistro
02 GET /api/admin/usuarios/1          -> 200 Admin, sin Password, ambos agregados en null
03 GET /api/admin/usuarios/2003       -> 200 Prestador (razonSocial y cuit presentes),
                                         vehiculos=0 choferes=0 viajes=0
04 GET /api/admin/usuarios/2          -> 200 Usuario, viajes=0, agregados en null
05 GET /api/admin/usuarios/999999     -> 404
06 escaneo de payloads                -> "password" ausente en lista y detalle
```

The EF log confirms the projection: `SELECT [u].[Id], [u].[Nombre], [u].[Apellido], [u].[Email], [u].[Rol], [u].[Estado], [u].[FechaRegistro] FROM [Usuarios] AS [u] ORDER BY [u].[FechaRegistro] DESC` — `Password` is never selected.

`dotnet build MundoRuta.slnx` — 0 errors, 150 warnings, the same count as before the feature.

### Post-smoke bugfix y hardening

El browser smoke real (Playwright + Edge del sistema, sesión sembrada en `localStorage`) reprodujo el fallo reportado: el modal mostraba el literal `_errorDetalle` aunque `GET api/admin/usuarios/{id}` respondía 200. Causa: en Razor, un parámetro de componente de tipo `string` con valor entrecomillado se pasa como **literal**; `Error="_errorDetalle"` enviaba el texto, no el valor del campo. Los parámetros `bool` y DTO no se veían afectados porque su valor se interpreta como expresión.

Correcciones:

- `Usuarios.razor` — `Error="@_errorDetalle"` (binding explícito).
- `Usuarios.razor` — `catch` defensivo en `CargarUsuariosAsync` y `AbrirDetalleAsync`, con mensaje neutro.
- `HttpRespExtensiones.ObtenerMensajeAsync` — ya no vuelca HTML crudo: ignora `text/html`, extrae `mensaje`/`message`/`error`/`detail`/`title` de JSON y limita el texto plano a 300 caracteres.
- `HttpServices` — los mensajes de excepción incluyen método, URL y tipo de excepción; se registran en `Console.Error`.
- `VehiculoDTO` / `ChoferDTO` — strings inicializados en `string.Empty`; `VehiculoDTO.FechaFabricación` pasa a `DateTime?`.

Verificación (Edge headless):

```
GET api/admin/usuarios            -> 200, 3 filas
Ver detalle (3 usuarios)          -> modal con datos, error=null, sin errores de consola
500 con HTML en el detalle        -> "Error interno del servidor" (sin HTML crudo)
fallo de red en el detalle        -> "No se pudo completar la solicitud. Detalle: GET api/admin/usuarios/2003 (HttpRequestException): ..."
buscar "juan"                     -> 1 fila; buscar "gmail.com" -> 2; chip Prestadores -> 1; estado APROBADO -> 3; 1 sola request a la lista
```

`dotnet build` — 0 errores. Warnings: baseline preexistente.

### Review

`gentle-ai review assess --base-ref 26923b4 --committed-only` after the backend batch returned `risk: medium`, `review_due: false`, `review_due_reason: under_budget` (263 changed lines). Receipt-driven development is enabled by global mode, so the slice stayed pending until the client batch pushed it to `review_due: true` / `slice_budget_reached` (968 changed lines). The native preflight ran and returned a `gentle-ai.review-integration.consent/v3` envelope, which was relayed unchanged; the user chose **declined**. The exact decline invocation returned `action: declined`, `consent: declined_this_candidate`, with the target identity matching, so no review record was created and reviews stay enabled for later changes. No push and no PR were made; delivery follows ordinary repository policy.

## Next Step

Los cambios del bugfix y del hardening están sin commitear. Decidir la forma de entrega: un PR con `size:exception` (~858 líneas autoradas) o las dos slices registradas (backend 232, cliente 626).
