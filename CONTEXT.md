# CONTEXT.md

Documento de contexto del proyecto **MundoRuta** para planificar y crear nuevas tareas.

---

## 1. Resumen y Stack Técnico

MundoRuta es una plataforma de fletes/viajes que conecta pasajeros y solicitantes con prestadores (empresas de transporte) y sus choferes, con un panel de administración.

Stack principal:
- **Backend:** .NET 10 / ASP.NET Core Web API (controllers) + Blazor Web App.
- **Frontend:** Blazor con render interactivo Server y WebAssembly (`MundoRuta.Server`, `MundoRuta.Server.Client`).
- **Base de datos:** Entity Framework Core 10 + SQL Server (LocalDB `MundoRuta` en dev).
- **Manejo de estado / sesiones:** NO implementado. No hay estado global, caché ni sesión en el cliente; cada request golpea la DB.
- **Estilos:** Bootstrap 5 (assets default de plantilla Blazor en `wwwroot/lib/bootstrap`).
- **Auth:** NO hay autenticación real. No hay JWT, cookies, Identity ni hashing de passwords.
- **Documentación API:** Swagger / Swashbuckle `10.2.3`.

Archivos de configuración críticos:
- `MundoRuta.slnx` (formato solución nueva, 6 proyectos).
- `MundoRuta.Server/MundoRuta.Server/appsettings.json` (connection string `ConnSqlServer` → LocalDB).
- `.editorconfig` (reglas de estilo del repo).

---

## 2. Arquitectura y Estructura de Carpetas

Patrón: **arquitectura en capas + repositorio genérico** (`Repositorio<E>` / `IRepositorio<E>`) con repositorios específicos, inyectados por DI en `Program.cs`.

```
mundo-ruta/
├── MundoRuta.Shared/            # DTOs compartidos Server ↔ Client
│   └── DTO/                     # LoginRequestDto, SolicitarViajeDTO, etc.
├── MundoRuta.BD/               # EF Core: contexto y entidades
│   ├── Datos/
│   │   ├── AppDbContext.cs      # DbSets + OnModelCreating (FKs, DeleteBehavior)
│   │   ├── EntidadBase.cs       # clase base con Id
│   │   └── Entity/              # 12 entidades (Usuario, Viaje, Chofer, ...)
│   └── Migrations/              # ~13 migraciones EF
├── MundoRuta.Repositorio/      # Acceso a datos (patrón repositorio)
│   └── Repositorios/           # Repositorio<E> + repos concretos e interfaces
├── MundoRuta.Servicios/        # VACÍO (solo Class1.cs) — reservado para capa de servicios Client
├── MundoRuta.Server/
│   ├── MundoRuta.Server/        # Host + API (Program.cs, Controllers/)
│   │   └── Controllers/         # Auth, Usuario, Pasajero, Prestador, Admin, Servicios, Calificaciones
│   └── MundoRuta.Server.Client/ # Blazor WebAssembly (plantilla default)
└── CONTEXT.md
```

- `MundoRuta.Shared`: modelos/DTOs sin dependencias de EF; referencia base de todos.
- `MundoRuta.BD`: solo EF/entidades; depende de `Shared`.
- `MundoRuta.Repositorio`: queries EF; depende de `BD` y `Shared`.
- `MundoRuta.Server`: host + controllers + DI; depende de `Client`, `Shared`, `BD`, `Repositorio`.
- `MundoRuta.Server.Client`: frontend WASM; referencia `Servicios` y `Shared`.

---

## 3. Modelo de Datos y Estado

`AppDbContext` expone 12 `DbSet`: `Usuarios`, `Servicios`, `PrestadorServicios`, `Liquidaciones`, `Choferes`, `Vehiculos`, `Viajes`, `Cargas`, `Pagos`, `Calificaciones`, `Chats`, `Mensajes`.

Entidades y relaciones (resumen):
- **Usuario** (`Usuario`): `Nombre`, `Apellido`, `Email`, `Password` (texto plano), `Telefono`, `FechaRegistro`, `Estado`, `Rol`, `RazonSocial`, `Cuit`. Roles usados: `Usuario`, `Prestador`, `Admin`, `Chofer`. Estados: `APROBADO`, `PENDIENTE`.
- **Viaje**: núcleo del dominio. Tiene 4 FKs a `Usuario` (`IdSolicitante`, `IdPasajero`, `IdUsuario`, más 1:1 conceptual) y referencias a `Chofer`, `Servicio`, `Vehiculo`. Campos: `Origen`, `Destino`, `TipoSolicitud`, `Estado`, `EstadoPago`, `Fecha`, `Hora`, `Monto`, `MontoEstimado`, `MetodoDePago`, `EquipajeCarga`, `DetalleCarga`, `MotivoCancelacion`, `FechaHoraReserva`.
- **Chofer**: `Licencia`, `Estado`, FK `IdUsuario` (→Usuario) y `IdPrestador` (→Usuario prestador).
- **Vehiculo**: `Patente`, `Marca`, `Licencia`, `Estado`, datos técnicos; FK `IdUsuario` (obligatoria) y `IdChofer` (opcional).
- **Servicio**: `Nombre`, `Descripcion`, `Tipo`, `Estado`.
- **Carga**: `Tipo`, `PesoEstimado`, `Descripcion`, FK `IdViaje`.
- **Pago**: `Monto`, `FechaPago`, `MetodoPago`, `Estado`, FK `IdViaje`.
- **Liquidacion**: `MontoBruto`, `Comision`, `MontoNeto`, `Estado`, `Fecha`, FK `IdUsuario`.
- **Calificacion**: `Puntaje`, `Comentario`, FK `IdViaje`, `IdUsuario`, `IdUsuarioPrestador`.
- **Chat**: `FechaInicio`, `FechaFin`, y `IdViaje` tipado **como entidad `Viaje`** (no int).
- **Mensaje**: FK `IdChat`, `IdEmisor`/`IdDestinatario` (nullable), `Contenido`, `EnviadoEn`.
- **PrestadorServicio**: FK `IdUsuario` + `idServicio` (minúscula) — sin uso en código.

`OnModelCreating` define relaciones explícitas con `DeleteBehavior.Restrict` para Viaje (Usuario/Solicitante/Pasajero/Chofer), Chofer (Usuario/Prestador) y Vehiculo (Chofer/Usuario).

Estado global/sesión en la app: **inexistente**. No hay store, caché ni token; el cliente Blazor no consume la API todavía.

---

## 4. Módulos y Flujos Implementados (lo que YA funciona)

Endpoints reales (rutas tal cual en el código):

- **AuthController** (`api/auth`)
  - `POST register` → crea Usuario; `Prestador` queda `PENDIENTE`, `Usuario` `APROBADO`; valida email único.
  - `POST login` → valida email/password; rechaza `PENDIENTE` con 403. Devuelve `{ id, nombre, rol }` (sin token).
- **UsuarioController** (`api/usuario`)
  - `GET {id}` → `UsuarioDTO`; `PUT {id}` → actualiza perfil, valida email único.
- **PasajeroController** (`api/pasajero`)
  - `PUT viajes/{id}/cancelar` → cancela con regla de 30 min de anticipación + `MotivoCancelacion`.
  - `POST viajes/solicitudes` → crea Viaje (`InsertarConServicio`, crea servicio default si no existe).
  - `PUT viajes/{id}/responder-contraoferta` → acepta (`ACEPTADO`) o rechaza (`CANCELADO`).
  - `GET {usuarioId}/viajes?estado=` → listado (`ListadoViajeDTO`).
  - `GET {usuarioId}/viajes/{viajeId}` → detalle (`ViajeDetalladoDTO`) con pasajero/chofer/vehículo.
- **PrestadorController** (`api/prestador`)
  - `GET {prestadorId}/solicitudes` → pendientes del prestador.
  - `POST choferes` → alta de Chofer + Usuario (rol `Chofer`) en transacción.
  - `GET {prestadorId}/choferes` → listado (`ChoferDTO`).
  - `GET {id}/perfil-publico` → `PrestadorPerfilPublicoDTO` con vehículos.
  - `PUT viajes/{id}/responder` → `ACEPTAR`/`RECHAZAR`.
  - `PUT viajes/{id}/contraofertar` → `CONTRAOFERTADO` + nuevo monto.
  - `POST vehiculo` / `GET {id}/vehiculos`.
  - `PUT viajes/{id}/finalizar` → solo si `EN_CURSO`; marca `FINALIZADO` y libera chofer (`DISPONIBLE`).
  - `PUT viajes/{id}/registrar-pago` → `EstadoPago = PAGADO`.
- **AdminController** (`api/admin`)
  - `GET prestadores-pendientes`, `PUT aprobar-prestador/{id}`, `GET dashboard` (totales de usuarios/prestadores/viajes finalizados).
- **ServiciosController** (`api/servicios`)
  - `GET fletes` → prestadores `APROBADO`.
- **CalificacionesController** (`api`)
  - `POST pasajero/calificaciones` (solo si el viaje está `FINALIZADO`).
  - `GET prestador/{IdPrestador}/calificaciones` → promedio, total y comentarios.

Persistencia: toda vía `MundoRuta.Repositorio`. `Program.cs` siembra un usuario admin (`admin@mundoruta.com` / `Admin123!`) si no existe.

Frontend: **solo la plantilla Blazor default** — `Pages/Home.razor`, `Counter.razor`, `Weather.razor`, `NotFound.razor`. No hay pantallas del dominio (login, viajes, prestadores, admin).

---

## 5. Trabajo en Progreso y Deuda Técnica (lo que FALTA o está a medias)

Incompleto / sin exponer:
- `MundoRuta.Servicios` está vacío (`Class1.cs`) pese a que el Client lo referencia: no existe la capa de servicios HTTP del frontend.
- `Class1.cs` sobrantes en `MundoRuta.Shared` y `MundoRuta.BD`.
- Repos `ChatRepositorio`, `MensajeRepositorio`, `CargaRepositorio`, `ServicioRepositorio` se registran en DI (`Program.cs`) pero **ningún controller** los usa → mensajería y cargas no expuestas. Existen DTOs listos: `CrearMensajeDTO`, `MensajeDTO`.
- Entidades `Pago` y `Liquidacion` tienen `DbSet` pero **sin repositorio ni controller**.
- Entidad `PrestadorServicio` sin uso alguno.
- `NotificacionRepositorio.cs` está **comentado** y no existe entidad `Notificacion` (solo la interfaz `INotificacionRepositorio`).

TODOs / FIXMEs: no hay marcadores literales en el código; la deuda está implícita en lo anterior.

Bugs / puntos débiles:
- **Estados inconsistentes:** `PrestadorController` escribe `"Aceptado"`/`"Rechazado"` (capitalización mixta) mientras el resto usa mayúsculas (`ACEPTADO`, `PENDIENTE`, `CONTRAOFERTADO`, `EN_CURSO`, `FINALIZADO`). Rompe comparaciones de estado.
- **Seguridad:** passwords en texto plano y sin hashing; sin `[Authorize]` ni verificación de rol (IDOR: los endpoints confían en el `id`/`usuarioId` de la URL); admin con password hardcodeada `Admin123!`; connection string en `appsettings.json`.
- **Lógica duplicada:** `ResponderSolicitud`, `ContraOfertar` y `RegistrarPago` existen tanto en `ViajeRepositorio` como re-implementados en `PrestadorController`.
- `app.MapControllers()` llamado dos veces en `Program.cs` (líneas 68 y 82).
- **Sin validación de DTOs** (no hay DataAnnotations ni FluentValidation); varios DTOs no llevan `[FromBody]`.
- `Chat.IdViaje` es de tipo `Viaje` en vez de FK `int IdViaje` (modelo inconsistente; `ChatRepositorio` depende de `c.IdViaje != null`).
- `VehiculoRepositorio.ObtenerPorPrestador` filtra por relación con `Chofer`, pero `POST vehiculo` permite crear vehículos sin chofer → pueden quedar invisibles.
- Sin proyecto de tests.
- `MundoRuta.Server.Client/Program.cs` no registra HttpClient ni servicios; el frontend no llama a la API.

---

## 6. Reglas de Código del Proyecto

Del `.editorconfig` (root, `[*.cs]`):
- Indent 4 espacios, `crlf`, sin newline final obligatorio.
- Namespaces **block-scoped** (`csharp_style_namespace_declarations = block_scoped`).
- `using` **fuera** del namespace (`outside_namespace`).
- Tipos y miembros públicos en **PascalCase**; interfaces con prefijo `I`.
- `var` desaconsejado (`csharp_style_var_* = false`).
- Preferir object/collection initializers, coerción, `required` y nullable habilitados.

Convenciones observadas en el repo:
- DTOs en `MundoRuta.Shared.DTO` (algunos en `MundoRuta.Shared/DTO` con namespace `MundoRuta.Shared.DTO`).
- Repositorios con métodos `async/await` y queries EF; interfaz por cada repo.
- Rutas API `api/{recurso}` con atributos `[ApiController]` y `[Route]`.
- Comentarios en español; mensajes de error en español.
- Una clase/entidad por archivo en `BD/Datos/Entity`.

---

## 7. Próximos Pasos Sugeridos (Backlog)

1. **Construir el frontend del dominio** en `MundoRuta.Server.Client`: páginas de login/registro y dashboards por rol (Usuario, Prestador, Admin, Chofer) que consuman los controllers existentes.
2. **Implementar la capa `MundoRuta.Servicios`**: servicios tipados con `HttpClient` (uno por dominio), registro en DI de `Client/Program.cs`, y eliminar `Class1.cs`.
3. **Autenticación y autorización real**: hashing de passwords (BCrypt/Identity), tokens, `[Authorize]` con políticas por rol y quitar el admin hardcodeado.
4. **Exponer mensajería/chat** con `ChatRepositorio`, `MensajeRepositorio` y los DTOs ya creados (`CrearMensajeDTO`, `MensajeDTO`) + UI de chat.
5. **Unificar estados de Viaje** en constantes/enum (eliminar la mezcla `Aceptado` vs `ACEPTADO`) y usarlos en controllers y repos.
6. **Validación y manejo de errores**: DataAnnotations/FluentValidation en DTOs y respuestas de error consistentes.
7. **Agregar proyecto de tests** (unitarios de repositorios/controllers) y limpiar código muerto (`Pago`/`Liquidacion`/`PrestadorServicio` sin uso, `NotificacionRepositorio` comentado, `MapControllers` duplicado).
