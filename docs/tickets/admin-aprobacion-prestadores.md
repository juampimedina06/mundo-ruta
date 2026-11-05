# Ticket: Bandeja de solicitudes de prestadores

Ruta Blazor: sección dentro de `/admin/dashboard`, debajo de las métricas (decisión del usuario: no se crea una página aparte).

Endpoints API:

- `GET /api/admin/prestadores/solicitudes`
- `PUT /api/admin/prestadores/{id}/estado`

## Descripción

Implementar la bandeja donde el Administrador evalúa a las empresas de transporte que se registraron en la plataforma (nacen en estado `PENDIENTE` y tienen el login bloqueado con `403 Forbidden`) para aprobarlas o rechazarlas.

## Criterios de Aceptación

- Consume `GET /api/admin/prestadores/solicitudes` y lista las solicitudes de alta de prestadores mostrando: Razón Social / Nombre, CUIT, Correo Electrónico, Teléfono, Fecha de solicitud (`dd/mm/aaaa`) y estado actual.
- Incluye barra de búsqueda y filtro rápido por estado (`PENDIENTE`, `APROBADO`, `RECHAZADO` / `SUSPENDIDO`).
- Acción Aprobar: botón que invoca `PUT /api/admin/prestadores/{id}/estado` enviando el nuevo estado `APROBADO` (habilitando inmediatamente a la empresa para iniciar sesión y operar).
- Acción Rechazar / Suspender: botón que invoca `PUT /api/admin/prestadores/{id}/estado` con estado `RECHAZADO` o `SUSPENDIDO` (con confirmación previa o motivo).
- Actualiza la grilla dinámicamente sin recargar toda la página al procesar cada solicitud.

## Guía técnica (buenas prácticas)

Extender `IAdminServicio` en `MundoRuta.Servicios` con la consulta de solicitudes y la actualización de estado, e inyectarlo en `Pages/Admin/Dashboard.razor`. La bandeja se renderiza como una sección del dashboard, reutilizando `EstadoBadge`, `EmptyState` y `FormatoPanel`.
