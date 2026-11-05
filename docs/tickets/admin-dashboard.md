# Ticket: Panel de Dashboard del Administrador

Ruta Blazor: `/admin/dashboard`

Endpoint API: `GET /api/admin/dashboard`

## Descripción

Desarrollar el panel principal de control del administrador donde se visualizan los indicadores clave (KPIs) de funcionamiento de la plataforma Mundo Ruta en tiempo real.

## Criterios de Aceptación

- Al cargar la vista, ejecuta `GET /api/admin/dashboard` a través de `IAdminServicio` (en `MundoRuta.Servicios`).
- Renderiza tarjetas de resumen (KPIs) claras mostrando:
  - Total de Usuarios registrados (desglosado por Pasajeros, Prestadores y Choferes).
  - Prestadores Pendientes de aprobación (con acceso rápido a la bandeja de solicitudes).
  - Viajes Totales y Viajes Activos (en curso en cualquiera de los 5 estados: buscando, asignado, carga-subida, en-transito, llegada-destino).
  - Volumen transaccionado / Comisiones generadas en el sistema.
- Estandariza cualquier fecha de corte o actividad reciente al formato `dd/mm/aaaa`.

## Guía técnica (buenas prácticas)

Crear la interfaz `IAdminServicio` y su implementación `AdminServicio` en el proyecto `MundoRuta.Servicios` e inyectarla en los componentes `.razor` del administrador.
