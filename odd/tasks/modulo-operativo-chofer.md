# Feature: Chofer — Módulo Operativo y Despacho de Viajes

## Objective

Implementar el flujo operativo completo para el perfil Conductor/Chofer en Mundo Ruta:
- Backend: Exponer y corregir los endpoints de la máquina de estados del viaje, validación estricta de DNI del receptor al finalizar la entrega, consulta de viaje activo, historial y perfil del conductor.
- Servicios: Encapsular el cliente HTTP en `IChoferServicio` dentro de `MundoRuta.Servicios`.
- Frontend: Construir las pantallas en Blazor WebAssembly (`/chofer/login`, `/chofer/home`, `/chofer/viaje-activo/{id}`, `/chofer/historial`, `/chofer/perfil`) manteniendo una interfaz simple, funcional y robusta.

## Problem & Current State Analysis

Al contrastar los tickets con la base de código existente se identificaron las siguientes discrepancias y deudas técnicas:

1. **Bug crítico de validación en `ChoferController.ValidarEntrega`**:
   - El código existente comparaba `viaje.Usuario.Cuit != dto.CuitIngresado`. `viaje.Usuario` representa a la empresa prestadora, no al receptor de la carga.
   - La entidad `Viaje` ya cuenta con el campo canónico `DniReceptor` (string).
   - El DTO `ValidarEntregaDTO` exponía `CuitIngresado` en lugar de `DniIngresado`.
   - **Ajuste**: Reemplazar/homogeneizar `ValidarEntregaDTO` con `DniIngresado` y validar estrictamente `viaje.DniReceptor == dto.DniIngresado`. Al finalizar, marcar `viaje.CalificadoPorChofer = true`, `viaje.Estado = EstadosViaje.Finalizado`, `chofer.Estado = "DISPONIBLE"` y crear la entidad `Calificacion` para el pasajero.

2. **Resolución de Identidad: `Usuario.Id` vs `Chofer.Id`**:
   - Cuando un usuario chofer inicia sesión en `/api/auth/login`, el backend devuelve el `Usuario.Id`.
   - La tabla `Choferes` tiene su propio `Id` autoincremental y un campo `IdUsuario` que referencia al usuario de autenticación.
   - En la entidad `Viaje`, la clave foránea es `v.IdChofer` (que apunta a `Chofer.Id`).
   - **Ajuste**: Los endpoints del controlador que reciben `{choferId}` deben resolver la entidad `Chofer` buscando tanto por `c.Id == choferId` como por `c.IdUsuario == choferId`. Esto blinda la API ante llamadas desde la sesión del frontend sin requerir malabares de almacenamiento en el cliente.

3. **Proyección y Contratos DTO en Backend**:
   - `GET api/chofer/{choferId}/viaje-activo` retornaba la entidad `Viaje` directa, corriendo riesgo de ciclos circulares de Entity Framework y omitiendo datos enriquecidos (nombre y teléfono del pasajero, especificaciones de carga legibles).
   - `GET api/chofer/{choferId}/perfil` devolvía un objeto anónimo sin incluir la razón social de la empresa prestadora a la que pertenece.
   - **Ajuste**: Tipar las respuestas con DTOs en `MundoRuta.Shared` (`ViajeChoferDTO`, `ChoferPerfilDTO`, `ChoferViajeHistorialDTO`).

4. **Ausencia de Servicio en Capa de Clientes**:
   - En `MundoRuta.Servicios` no existía `IChoferServicio` ni `ChoferServicio`.
   - **Ajuste**: Centralizar las peticiones a `api/chofer/...` en `IChoferServicio` e inyectarlo en DI mediante `AddMundoRutaServicios`.

5. **Complejidad del Frontend**:
   - El requerimiento pide mantener el frontend simple. Se evitarán dependencias pesadas de librerías GIS externas; se creará un contenedor de mapa visualmente claro y limpio (placeholder estilizado con indicadores y coordenadas/direcciones legibles), enfocado en la usabilidad del chofer y transiciones de estado fluidas.

## Scope

- **Shared**:
  - `ValidarEntregaDTO`: incorporar `DniIngresado` (soportando fallback a `CuitIngresado` si fuera necesario para retrocompatibilidad), `PuntajePasajero`, `Comentario`.
  - `ViajeChoferDTO`: información completa del viaje en curso (id, origen, destino, pasajero, teléfono, esCubierto, usaBaulera, llevaAcompanante, estado, monto, dniReceptor).
  - `ChoferPerfilDTO`: datos personales, licencia, empresa prestadora, promedio de calificación, foto.
  - `ChoferViajeHistorialDTO`: id, origen, destino, monto, fecha formateada en `dd/MM/yyyy`.

- **Backend (`MundoRuta.Server` y `MundoRuta.Repositorio`)**:
  - `ChoferController`:
    - `GET /api/chofer/{choferId}/viaje-activo`: resuelve chofer, busca viaje en estados activos (`EstadosViaje.Activos`), retorna `ViajeChoferDTO`.
    - `PUT /api/chofer/viajes/{id}/avanzar-estado`: valida rigurosamente secuencia (`asignado` -> `carga-subida` -> `en-transito` -> `llegada-destino`), emite SignalR `EstadoViajeActualizado`.
    - `POST /api/chofer/viajes/{id}/validar-entrega`: compara DNI con `viaje.DniReceptor`. Pasa estado a `FINALIZADO`, chofer a `DISPONIBLE`, persiste calificación al pasajero y emite evento SignalR.
    - `GET /api/chofer/{choferId}/historial`: lista viajes finalizados proyectados a `ChoferViajeHistorialDTO` ordenados cronológicamente descendente.
    - `GET /api/chofer/{choferId}/perfil`: datos de chofer + razón social del prestador + promedio ponderado de calificaciones.
    - `PUT /api/chofer/{choferId}/perfil`: actualización de teléfono y `FotoPerfil`.

- **Servicios (`MundoRuta.Servicios`)**:
  - `IChoferServicio` / `ChoferServicio` en namespace `MundoRuta.Servicios.Chofer`.
  - Registro en `ServiceCollectionExtensions.AddMundoRutaServicios`.

- **Frontend (`MundoRuta.Server.Client`)**:
  - `/chofer/login`: Redirección/pantalla simple de acceso para Conductor.
  - `/chofer/home` (y soporte a `/chofer`): Dashboard de espera con indicador "Disponible — Esperando asignación de viaje", contenedor de mapa simple, y detección de viaje asignado con redirección a `/chofer/viaje-activo/{id}`.
  - `/chofer/viaje-activo/{viajeId:int}`: Hoja de ruta dinámica con botón de acción contextual según el estado, panel de chat integrado (`/api/chat/viajes/{id}/mensajes`) y despliegue de formulario de validación al llegar a destino.
  - Componente/Sección de Cierre: Validación de DNI, calificación con estrellas (1 a 5) y comentario opcional.
  - `/chofer/historial`: Tabla/tarjetas de viajes completados con fecha estrictamente `dd/MM/yyyy`.
  - `/chofer/perfil`: Vista de datos, licencia, empresa, reputación y formulario para editar foto y contacto.

## Tasks & Model Routing

| Tarea | Descripción | Complejidad | Modelo Recomendado |
| :--- | :--- | :--- | :--- |
| **TASK-01** | Contratos DTO en `MundoRuta.Shared` (`ValidarEntregaDTO`, `ViajeChoferDTO`, `ChoferPerfilDTO`, `ChoferViajeHistorialDTO`) | Media / Contratos | **Inherit / Pro** |
| **TASK-02** | Refactor y corrección de `ChoferController.cs` (Validación DNI, resolución Usuario/Chofer, estados y SignalR) | Alta / Lógica Crítica | **Inherit / Pro** |
| **TASK-03** | Creación de `IChoferServicio` y `ChoferServicio` en `MundoRuta.Servicios` y registro DI | Baja / Repetitiva | **Flash** |
| **TASK-04** | Pantallas de Acceso y Espera (`/chofer/login`, `/chofer/home` con mapa simple y polling/evento) | Media / UI Blazor | **Flash** |
| **TASK-05** | Flujo Operativo Dinámico `/chofer/viaje-activo/{viajeId}` (Botonera según máquina de estados + Chat) | Alta / Interactividad | **Inherit / Pro** |
| **TASK-06** | Modal/Formulario de Cierre de Viaje (Validación DNI, feedback de error y calificación al cliente) | Media / Formulario | **Flash** |
| **TASK-07** | Pantallas `/chofer/historial` y `/chofer/perfil` (Listado con fecha `dd/MM/yyyy` y edición de datos) | Baja / Vistas CRUD | **Flash** |
| **TASK-08** | Smoke tests de endpoints, compilación y verificación integral | Media / Verificación | **Inherit / Pro** |

## Acceptance Criteria

- [x] `dotnet build MundoRuta.slnx` compila con 0 errores.
- [x] `GET /api/chofer/{choferId}/viaje-activo` resuelve correctamente tanto si se le pasa el `Chofer.Id` como el `Usuario.Id` de la sesión.
- [x] `PUT /api/chofer/viajes/{id}/avanzar-estado` avanza en orden estricto (`ASIGNADO` -> `CARGA_SUBIDA` -> `EN_TRANSITO` -> `LLEGADA_DESTINO`) y rechaza saltos de estado con 400 Bad Request.
- [x] `POST /api/chofer/viajes/{id}/validar-entrega` valida el `DniIngresado` contra el `DniReceptor` del viaje. Si no coincide, retorna 400 ("DNI inválido") y no modifica estados. Si coincide, finaliza el viaje, marca al chofer como `DISPONIBLE` y registra la calificación.
- [x] `GET /api/chofer/{choferId}/historial` lista los viajes finalizados con fechas formateadas en `dd/MM/yyyy`.
- [x] `GET /api/chofer/{choferId}/perfil` retorna datos del chofer, promedio de calificación y razón social de la empresa prestadora.
- [x] En `/chofer/home`, el chofer visualiza su estado disponible y al tener un viaje asignado puede ingresar directamente a la pantalla operativa.
- [x] En `/chofer/viaje-activo/{viajeId}`, los botones cambian dinámicamente y el chat permite comunicarse con el pasajero.
- [x] Al llegar a destino, la pantalla exige validar el DNI y calificar antes de concluir el servicio.

## Tasks Execution Status

- [x] TASK-01: Shared contracts (`ValidarEntregaDTO`, `ViajeChoferDTO`, `ChoferPerfilDTO`, `ChoferViajeHistorialDTO`).
- [x] TASK-02: Refactor y corrección de `ChoferController.cs` (Validación DNI, resolución Usuario/Chofer, estados y SignalR).
- [x] TASK-03: Creación de `IChoferServicio` y `ChoferServicio` en `MundoRuta.Servicios` y registro DI.
- [x] TASK-04: Pantallas de Acceso y Espera (`/chofer/login`, `/chofer/home` con mapa simple y polling/evento).
- [x] TASK-05: Flujo Operativo Dinámico `/chofer/viaje-activo/{viajeId}` (Botonera según máquina de estados + Chat).
- [x] TASK-06: Modal/Formulario de Cierre de Viaje (Validación DNI, feedback de error y calificación al cliente).
- [x] TASK-07: Pantallas `/chofer/historial` y `/chofer/perfil` (Listado con fecha `dd/MM/yyyy` y edición de datos).
- [x] TASK-08: Smoke tests de compilación integral (0 errores en los 5 proyectos).
