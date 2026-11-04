# Ticket — Directorio de usuarios (admin)

Transcrito desde `MundoRuta.Shared/ticketEliminar.cs`, que contenía texto plano dentro de un archivo `.cs` y rompía el build.

## Ruta Blazor

`/admin/usuarios`

## Endpoints API

- `GET /api/admin/usuarios`
- `GET /api/admin/usuarios/{id}`

## Descripción

Construir el directorio centralizado de todos los usuarios registrados en el sistema para tareas de soporte, auditoría y consulta de perfiles.

## Criterios de Aceptación

- Consume `GET /api/admin/usuarios` y presenta una tabla limpia con: Nombre y Apellido, Email, Rol (Usuario, Prestador, Chofer, Admin), Estado de la cuenta y Fecha de alta (dd/mm/aaaa).
- Incorpora buscador por texto (nombre o correo) y filtro por Rol y Estado.
- Cada fila cuenta con un botón "Ver detalle" que consume `GET /api/admin/usuarios/{id}` y despliega un modal con la ficha completa del usuario (datos de contacto, promedio de calificación, cantidad de viajes o flota asociada si es prestador).
