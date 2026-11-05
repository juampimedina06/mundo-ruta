# Ticket — Auditoría de viajes y comisiones (admin)

## Ruta Blazor

`/admin/comisiones`

## Endpoints API

- `GET /api/admin/comisiones`

## Descripción

Desarrollar la pantalla de seguimiento financiero y operativo donde el administrador audita los viajes cerrados, el monto final acordado en la subasta/viaje y la comisión correspondiente a la plataforma.

## Criterios de Aceptación

- Consume `GET /api/admin/comisiones` y lista el desglose de operaciones con: ID de Viaje, Prestador asignado, Pasajero, Fecha (dd/mm/aaaa), Monto total del flete, Porcentaje/Monto de comisión de Mundo Ruta y Estado del pago/comisión.
- Muestra totalizadores en la parte superior (Total facturado en fletes vs. Total de comisiones retenidas/pendientes).
- Incluye filtros por rango de fechas (dd/mm/aaaa), por empresa prestadora y por estado del viaje (buscando, asignado, carga-subida, en-transito, llegada-destino, finalizado, cancelado).
