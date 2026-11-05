# Feature: Admin — Auditoría de viajes y comisiones

## Objective

Build the financial and operational audit screen at Blazor route `/admin/comisiones`, backed by `GET api/admin/comisiones`, allowing the administrator to inspect transacted trips, agreed amounts, platform commission breakdown, and top-level KPIs with real-time filters.

## Problem

`AdminController` lacked a commission/trip audit endpoint. There was no query model or DTO for the commission breakdown with top-level financial aggregates, no repository method to query trips with passenger and provider details filtered by date range, provider and state, no method in `IAdminServicio`, and no `/admin/comisiones` page.

## Scope

- Move ticket definition to `docs/tickets/admin-comisiones.md`.
- DTOs in `MundoRuta.Shared`:
  - `AdminComisionFilaDTO`
  - `AdminComisionesReporteDTO`
  - `AdminFiltroComisionesDTO`
- Repository extension in `MundoRuta.Repositorio`:
  - `IViajeRepositorio.ObtenerReporteComisiones(AdminFiltroComisionesDTO filtro, decimal porcentajeComision)`
- Commission configuration:
  - Read `COMISION_PLATAFORMA_PORCENTAJE` from `ConfiguracionesSistema` with defensive fallback to `10%`.
- Controller endpoint in `MundoRuta.Server`:
  - `GET api/admin/comisiones` with query parameters.
- Client service layer in `MundoRuta.Servicios`:
  - `IAdminServicio.ObtenerComisionesAsync(AdminFiltroComisionesDTO? filtro = null)`
- Blazor page in `MundoRuta.Server.Client`:
  - `Pages/Admin/Comisiones.razor` with top KPI totals, reactive filters (date range, provider, trip state), and clean table.
- Navigation:
  - Add link card in `Pages/Admin/Panel.razor`.
  - Add menu item in `Navigation/MenuPorRol.cs`.

## Constraints

- Work on branch `feature/admin-auditoriaViajes`.
- Follow `.editorconfig`: block-scoped namespaces, usings outside namespace, avoid `var`, CRLF, nullable enabled.
- `MundoRuta.Servicios` references only `Shared`.
- Return 200 OK with empty breakdown when no trips match, never 404.
- Money values formatted with `FormatoPanel.Moneda`; dates formatted with `FormatoPanel.Fecha` (`dd/MM/yyyy`).

## Acceptance Criteria

- [x] Consume `GET /api/admin/comisiones` and list operation breakdown with: Trip ID, assigned Provider, Passenger, Date (dd/mm/yyyy), total amount, percentage/amount of Mundo Ruta commission and payment/commission status.
- [x] Top KPI totalizers (Total facturado en fletes vs. Total de comisiones retenidas/pendientes).
- [x] Filters by date range (dd/mm/yyyy), provider company, and trip status (buscando, asignado, carga-subida, en-transito, llegada-destino, finalizado, cancelado).

## Tasks

- [x] TASK-01: Create DTOs in `MundoRuta.Shared`.
- [x] TASK-02: Implement repository method in `IViajeRepositorio` and `ViajeRepositorio`.
- [x] TASK-03: Implement `GET api/admin/comisiones` in `AdminController`.
- [x] TASK-04: Implement client service in `IAdminServicio` and `AdminServicio`.
- [x] TASK-05: Create `Pages/Admin/Comisiones.razor`, update `Pages/Admin/Panel.razor` and `MenuPorRol.cs`.
- [x] TASK-06: Verification & build check.

## Checks

- [x] `dotnet build MundoRuta.slnx` — 0 errors.
- [x] `GET http://127.0.0.1:5090/api/admin/comisiones` — 200 OK with structured payload (`totalFacturado`, `totalComisionesRetenidas`, `totalComisionesPendientes`, `cantidadOperaciones`, `operaciones`).
- [x] Defensive handling of `ConfiguracionesSistema` in case table is not migrated in local DB.
- [x] Blazor client compilation with RoleGuard and FormatoPanel formatting.

## Evidence

### Files changed

Added:
- `docs/tickets/admin-comisiones.md`
- `odd/tasks/admin-comisiones.md`
- `MundoRuta.Shared/DTO/AdminComisionFilaDTO.cs`
- `MundoRuta.Shared/DTO/AdminComisionesReporteDTO.cs`
- `MundoRuta.Shared/DTO/AdminFiltroComisionesDTO.cs`
- `MundoRuta.Server/MundoRuta.Server.Client/Pages/Admin/Comisiones.razor`

Modified:
- `MundoRuta.Repositorio/Repositorios/IViajeRepositorio.cs`
- `MundoRuta.Repositorio/Repositorios/ViajeRepositorio.cs`
- `MundoRuta.Server/MundoRuta.Server/Controllers/AdminController.cs`
- `MundoRuta.Servicios/Admin/IAdminServicio.cs`
- `MundoRuta.Servicios/Admin/AdminServicio.cs`
- `MundoRuta.Server/MundoRuta.Server.Client/Pages/Admin/Panel.razor`
- `MundoRuta.Server/MundoRuta.Server.Client/Navigation/MenuPorRol.cs`
