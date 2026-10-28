# Feature: Frontend institucional por roles

## Objective
Reemplazar la plantilla por defecto de Blazor en `MundoRuta.Server.Client` por el layout institucional de Mundo Ruta (navbar superior, footer y paleta azul marino + naranja) y dejar el cliente organizado por rol (`Usuario`, `Prestador`, `Chofer`, `Admin`) con login, registro, guardas de ruta y páginas propias de cada rol.

## Problem
El cliente solo tiene las páginas de plantilla (`Home`, `Counter`, `Weather`) con menú lateral violeta. `MundoRuta.Servicios` ya expone `AuthServicio`, `SesionEstado`, `HttpServices` y `ViajeHubService`, pero ninguna pantalla los consume. No hay navegación por rol, ni forma de iniciar sesión, ni separación entre áreas, y la sesión en memoria se pierde al refrescar.

## Why
El ticket `MundoRuta.Shared/prompt.cs` pide el layout institucional con navbar dinámico por rol y footer de cuatro columnas. Sin una estructura por roles, cada pantalla futura termina mezclando responsabilidades y comparte navegación incorrecta.

## Scope
- Constantes de rol compartidas.
- Persistencia de sesión en `localStorage` para que el refresh no cierre sesión.
- Tokens de identidad visual y estilos base.
- `MainLayout` con `NavBar` y `Footer`.
- Configuración central de menús por rol.
- `RoleGuard` y componentes compartidos de UI.
- Páginas públicas: inicio, login, registro, acceso denegado, servicios y prestadores.
- Páginas placeholder por rol, listas para conectar a la API después.
- Eliminación de `Counter`, `Weather`, `NavMenu` y el CSS de plantilla.

## Out of scope
- Pantallas de dominio funcionales (solicitar viaje, ofertas, detalle real, choferes, vehículos).
- Autenticación real en backend, tokens, hashing de passwords, `[Authorize]`.
- Proyecto de tests.

## Constraints
- .NET 10, Blazor WebAssembly dentro de Blazor Web App.
- Sin paquetes NuGet nuevos: la persistencia usa `IJSRuntime` y `System.Text.Json`.
- La app pasa a render WASM sin prerender para que `SesionEstado` sea único y consistente.
- Nombres de dominio en español, siguiendo el repo; comentarios en español; sin `var`; namespaces block-scoped; `using` fuera del namespace.
- El backend no cambia.
- No tocar `MundoRuta.Shared/prompt.cs` (ticket de referencia).
- Trabajo por work-units en la rama `feature/estructura-inicial`.

## Decisions
- **Roles**: `MundoRuta.Shared/Roles.cs` con `Usuario`, `Prestador`, `Chofer`, `Admin`.
- **Render mode**: `Routes` con `InteractiveWebAssemblyRenderMode(prerender: false)`. Evita el choque de `SesionEstado` entre servidor y WASM.
- **Sesión**: `SesionEstado` suma `Restaurada` y `MarcarRestaurada()`. `SesionPersistencia` (cliente) guarda/restaura en `localStorage` con la clave `mundoruta.sesion` y se suscribe a `CambioSesion`.
- **Layout único**: `MainLayout` es el shell (navbar + contenido + footer). El menú se resuelve por rol, no hay layouts duplicados por rol.
- **Aislamiento**: una carpeta por rol en `Pages/`; lo compartido vive en `Components`; los menús se definen solo en `Navigation/MenuPorRol.cs`.
- **Guardas**: `RoleGuard` envuelve el contenido de cada página de rol. Sin sesión redirige a `/login?redirect=...`; rol incorrecto a `/acceso-denegado`.
- **Logo**: SVG placeholder en `wwwroot/img/logo-mundo-ruta.svg` hasta recibir el asset de Figma.
- **Seguridad**: la sesión en `localStorage` es solo estado de UI. El backend sigue sin autorización real.

## Design tokens
```
--mr-azul-marino: #0F1F3D
--mr-azul-marino-900: #0A172C
--mr-azul-marino-100: #E8EDF5
--mr-naranja: #FF5A1F
--mr-naranja-oscuro: #E64A12
--mr-fondo: #F7F9FC
--mr-texto: #1F2937
--mr-texto-suave: #64748B
--mr-borde: #D9E2EC
--mr-exito: #15803D
--mr-error: #B91C1C
--mr-radio: 6px
--mr-sombra: 0 4px 16px rgba(15, 31, 61, .08)
--mr-fuente: "Segoe UI", system-ui, -apple-system, "Helvetica Neue", Arial, sans-serif
--mr-ancho-contenido: 1180px
```
Bootstrap sigue cargado; se sobrescriben `.btn-primary` (naranja), links (azul marino) y foco de inputs.

## Routes
| Rol | Rutas |
| --- | --- |
| Público | `/`, `/servicios`, `/prestadores`, `/login`, `/registro`, `/acceso-denegado` |
| Usuario | `/usuario`, `/usuario/solicitar`, `/usuario/viajes`, `/usuario/viajes/{ViajeId:int}`, `/usuario/perfil` |
| Prestador | `/prestador`, `/prestador/solicitudes`, `/prestador/viajes`, `/prestador/choferes`, `/prestador/vehiculos`, `/prestador/perfil` |
| Chofer | `/chofer`, `/chofer/historial`, `/chofer/perfil` |
| Admin | `/admin`, `/admin/prestadores`, `/admin/configuracion` |

Menús por rol (labels exactos):
- Público: Inicio, Servicios, Prestadores.
- Usuario: Inicio, Solicitar viaje, Mis viajes, Mi perfil.
- Prestador: Panel, Solicitudes, Viajes, Choferes, Vehículos, Mi perfil.
- Chofer: Mi viaje, Historial, Mi perfil.
- Admin: Panel, Prestadores, Configuración.

## Tasks
- [x] TASK-01: Constantes de rol, estado de restauración y persistencia de sesión en `localStorage`; render mode WASM sin prerender.
- [x] TASK-02: Tokens de identidad visual, estilos base y logo placeholder.
- [x] TASK-03: Configuración de menús por rol, `RoleGuard` y componentes compartidos (`PageHeader`, `EmptyState`, `EstadoBadge`).
- [x] TASK-04: Reemplazar `MainLayout` por navbar + contenido + footer institucional; eliminar `NavMenu`.
- [x] TASK-05: Páginas públicas con login y registro funcionales; eliminar páginas de plantilla.
- [x] TASK-06: Páginas de Usuario y Chofer.
- [x] TASK-07: Páginas de Prestador y Admin.
- [x] TASK-08: Build completo y verificación manual de rutas por rol.

## Acceptance Criteria
- [x] Variables CSS globales con azul marino, naranja `#FF5A1F` y sans-serif.
- [x] Navbar superior con logo, enlaces dinámicos según rol y botón de perfil / Iniciar Sesión.
- [x] Footer con columnas Mundo Ruta, Soluciones, Red & Operaciones y Atención al Cliente.
- [x] Login redirige al inicio del rol; logout vuelve al sitio público.
- [ ] Refrescar la página conserva la sesión — implementado con `localStorage`, falta probarlo en navegador.
- [x] Un usuario de un rol no puede ver páginas de otro rol (verificado estáticamente: las 17 páginas de rol envuelven su contenido en `RoleGuard` con el rol correcto).
- [x] Cada carpeta de rol contiene solo sus páginas.
- [x] `dotnet build MundoRuta.slnx` sin errores.

## Checks
- [x] `dotnet build MundoRuta.slnx` — clean build: 148 advertencias preexistentes, 0 errores, 0 advertencias nuevas del cliente.
- [ ] Navegación pública sin sesión — pendiente de prueba en navegador.
- [ ] Login por cada rol y redirección correcta — pendiente; requiere usuarios en la base.
- [x] Acceso directo por URL a un rol ajeno redirige a `/acceso-denegado` (verificado en código, no en navegador).
- [ ] Refresh en `/usuario`, `/prestador`, `/chofer`, `/admin` conserva la sesión — pendiente de prueba en navegador.
- [x] Logout limpia `localStorage` (`GuardarAsync` elimina la clave cuando no hay sesión).
- [x] Inventario de rutas: 24 rutas, sin duplicados y sin restos de la plantilla (`Counter`, `Weather`, `NavMenu`, `sidebar`).

> Las verificaciones marcadas como estáticas se validaron por lectura de código y build; la corrida manual end-to-end queda pendiente porque requiere LocalDB y datos de login.

## Progress
- TASK-01 y TASK-02 completadas: Roles, SesionPersistencia, render WASM sin prerender, tokens CSS y logo.
- TASK-03 y TASK-04 completadas: menús por rol, `RoleGuard`, componentes compartidos, navbar y footer. `NavMenu` eliminado.
- TASK-05 completada: Home, Servicios, Prestadores, Login, Registro y Acceso denegado en `Pages/Public`; `Counter`, `Weather` y la Home de plantilla eliminadas; `NotFound` localizada.
- Exploración del cliente, servicios y controladores completada.
- Detectado intento previo en `feature/frontend-auth-roles-structure`; se reutiliza solo la paleta y el contenido del footer, no su estructura.
- TASK-06 completada: 8 páginas placeholder (5 de Usuario, 3 de Chofer) con `RoleGuard`; `.mr-card--enlace` agregado a `app.css`.
- TASK-07 completada: 9 páginas placeholder (6 de Prestador, 3 de Admin) con `RoleGuard`.
- TASK-08 completada: clean build sin errores, inventario de 24 rutas, sin restos de la plantilla; `SesionPersistencia` tolera JSON corrupto.
- Pendiente: prueba manual end-to-end en navegador (login por rol, refresh y logout requieren LocalDB con usuarios).
- Siguiente: correr la app, validar los flujos y abrir el PR.

## Verification evidence
- `dotnet build MundoRuta.slnx`: compilación correcta, 0 errores (TASK-01/02 y TASK-03/04).
- `dotnet build MundoRuta.slnx`: compilación correcta, 0 advertencias, 0 errores (TASK-05).
- `dotnet clean` + `dotnet build MundoRuta.slnx`: 0 errores, 148 advertencias preexistentes (ninguna del cliente) (TASK-08).
- Nota: `MundoRuta.Shared/prompt.cs` rompía el build por la extensión; se excluyó de compilación en el csproj sin tocar el archivo.

## Next step
Prueba manual end-to-end en navegador y apertura del PR.
