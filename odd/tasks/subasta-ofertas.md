# Feature: Subasta de ofertas con límite 5 postulaciones

## Objective
Migrar el flujo de creación/cotización a modelo tipo subasta con límite de 5 postulaciones, recargo por acompañante configurable, tarifa por km en prestador, listar ofertas con flags y selección de oferta.

## Problem
Actualmente el flujo es directo (prestador responde/contraoferta) sin subasta. Falta entidad para ofertas, límites, flags para el pasajero y recargo configurable.

## Scope
- Unificar estados a MAYÚSCULAS
- Agregar recargo 15% por acompañante configurable vía endpoint Admin
- Opción A: TarifaBaseKm en Usuario (ya existe) para configurar tarifa por km
- Crear/Completar flujo de subasta: SolicitarViaje, OfertasViaje, listar ofertas con flags, seleccionar oferta
- Bloquear 6ta oferta
- Al finalizar tarea, generar PR

## Constraints
- Seguir patrón repositorio existente
- Usar transacciones para selección de oferta
- No tocar Chat.IdViaje
- Mantener compatibilidad donde sea posible

## Acceptance Criteria
- [ ] Estados unificados a MAYÚSCULAS (BUSCANDO, ASIGNADO, CONTRAOFERTADO, CANCELADO, EN_TRANSITO, FINALIZADO, etc.)
- [ ] Recargo 15% por acompañante aplicable y configurable por Admin vía endpoint
- [ ] Prestador puede configurar TarifaBaseKm vía PUT api/prestador/{id}/tarifa-km
- [ ] Límite 5 ofertas activas por viaje, 6ta bloqueada
- [ ] GET api/pasajero/viajes/{id}/ofertas devuelve ofertas con EsMasBarata, EsMejorCalificado, ordenadas por precio asc
- [ ] PUT api/pasajero/viajes/{id}/seleccionar-oferta/{ofertaId} asigna viaje, fija monto, rechaza otras ofertas
- [ ] Prestador crea oferta vía responder/contraofertar (registrando en OfertasViaje)

## Tasks
- [ ] TASK-01: Unificar estados a MAYÚSCULAS (EstadosViaje + reemplazos)
- [ ] TASK-02: Crear entidad/DTOs/config para recargo acompañante (ConfiguracionSistema) + endpoint Admin
- [ ] TASK-03: Ampliar SolicitarViajeDTO y aplicar recargo acompañante al crear viaje
- [ ] TASK-04: Endpoint PUT api/prestador/{id}/tarifa-km (usar TarifaBaseKm)
- [ ] TASK-05: Completar repositorio OfertasViaje con métodos necesarios
- [ ] TASK-06: Modificar Prestador responder/contraofertar para crear oferta con límite 5
- [ ] TASK-07: Crear GET api/pasajero/viajes/{id}/ofertas con flags y orden
- [ ] TASK-08: Crear PUT api/pasajero/viajes/{id}/seleccionar-oferta/{ofertaId} transaccional
- [ ] TASK-09: Pruebas manuales y correcciones
- [ ] TASK-10: Generar PR

## Checks
- [ ] Compila sin errores
- [ ] Migración aplicada si hay cambios BD
- [ ] Endpoints probados según criterios
- [ ] Estados consistentes
