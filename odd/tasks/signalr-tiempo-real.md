# Feature: SignalR tiempo real (ofertas, estados, chat)

## Objective
Configurar comunicación bidireccional en vivo con SignalR nativo para evitar recargas de pantalla en ofertas, cambios de estado y mensajes de chat. Todo estructurado por grupos/salas y emitiendo desde controllers. Al finalizar, eliminar `MundoRuta.Shared/Ticket.cs`.

## Problem
Hoy los cambios de estado/ofertas requieren recarga. Hay que notificar en tiempo real sin usar SignalR "a lo loco", respetando el patrón del repo y manteniendo los endpoints REST tal como están.

## Scope
- Crear `ViajeHub` + registro en `Program.cs`
- Definir grupos: `viaje_{id}`, `prestador_{id}`, `pasajero_{id}`, `chofer_{id}`, `chat_{idChat}`
- Emitir eventos desde `PrestadorController`, `PasajeroController` y `ChatController` (si aplica)
- DTOs de eventos en `MundoRuta.Shared/DTO/Realtime/`
- Conexión + suscripción en `MundoRuta.Server.Client` con `Microsoft.AspNetCore.SignalR.Client`
- Eliminar `MundoRuta.Shared/Ticket.cs` al cerrar
- No tocar `Chat.IdViaje`
- Emitir **después** de `SaveChangesAsync()`

## Constraints
- Seguir patrón repositorio/DI existente
- Usar `IHubContext<ViajeHub>` inyectado por constructor
- Emitir solo por grupos involucrados
- Mantener lógica de negocio en controllers/repositorios, nada en Hub
- Sin añadir SignalR a Shared
- Rest puro sigue funcionando

## Acceptance Criteria
- [ ] Hub configurado y mapeado en `Program.cs`
- [ ] Grupos funcionales por contexto
- [ ] `NuevaOfertaRecibida` al crear oferta/contraoferta
- [ ] `ViajeAsignado` al seleccionar oferta
- [ ] `EstadoViajeActualizado` al avanzar/finalizar/pagar
- [ ] `NuevoMensajeChat` al enviar mensaje (si existe ChatController)
- [ ] Cliente se conecta, se une a grupos y escucha eventos sin F5
- [ ] Probado con 2 navegadores distintos
- [ ] `MundoRuta.Shared/Ticket.cs` eliminado

## Tasks
- [ ] TASK-01: Crear estructura Hub (Hubs/ViajeHub.cs) + registrar SignalR en Program.cs
- [ ] TASK-02: Crear DTOs de eventos en Shared/DTO/Realtime/
- [ ] TASK-03: Inyectar IHubContext en PrestadorController y emitir eventos
- [ ] TASK-04: Inyectar IHubContext en PasajeroController (SeleccionarOferta) y emitir
- [ ] TASK-05: Emitir en FinalizarServicio / RegistrarPago
- [ ] TASK-06: Integrar ChatController si existe (NuevoMensajeChat)
- [ ] TASK-07: Agregar paquete SignalR.Client a Server.Client + servicio/conexión + listeners
- [ ] TASK-08: Unirse a grupos desde UI/componentes necesarios
- [ ] TASK-09: Pruebas manuales con 2 navegadores
- [ ] TASK-10: Eliminar Ticket.cs + limpieza + commit final + PR

## Checks
- [ ] Compila sin errores (Server + Server.Client + Shared)
- [ ] No se rompe flujo REST existente
- [ ] Eventos solo llegan a grupos correctos
- [ ] Se emite después de SaveChangesAsync()
- [ ] Ticket.cs removido

## Commits
- feature/signalr-tiempo-real: trabajo por work-unit con conventional commits
