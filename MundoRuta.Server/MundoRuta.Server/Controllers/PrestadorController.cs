    // GET: Obtener un chofer por id
    [HttpGet("choferes/{choferId:int}")]
    [Authorize(Roles = "Prestador")]
    public async Task<IActionResult> ObtenerChoferPorId(int choferId)
    {
        try
        {
            var chofer = await _choferRepositorio.ObtenerPorIdAsync(choferId);

            if (chofer == null)
            {
                return NotFound(new { mensaje = "El chofer solicitado no existe." });
            }

            return Ok(chofer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
        }
    }

    // PUT: Modificar datos de un chofer
    [HttpPut("choferes/{choferId:int}")]
    [Authorize(Roles = "Prestador")]
    public async Task<IActionResult> ModificarChofer(int choferId, [FromBody] ChoferUpdateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var actualizado = await _choferRepositorio.ActualizarChoferAsync(choferId, dto);

            if (!actualizado)
            {
                return NotFound(new { mensaje = "No se pudo actualizar. El chofer no existe." });
            }

            return Ok(new { mensaje = "Datos del chofer actualizados correctamente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al actualizar el chofer", detalle = ex.Message });
        }
    }

    // DELETE: Baja lógica de un chofer
    [HttpDelete("choferes/{choferId:int}")]
    [Authorize(Roles = "Prestador")]
    public async Task<IActionResult> BajaChofer(int choferId)
    {
        try
        {
            var tieneViajesPendientes = await _choferRepositorio.TieneViajesEnTransitoAsync(choferId);

            if (tieneViajesPendientes)
            {
                return BadRequest(new { mensaje = "No se puede dar de baja. El chofer tiene viajes en tránsito asignados." });
            }

            var bajaExitosa = await _choferRepositorio.BajaLogicaAsync(choferId);

            if (!bajaExitosa)
            {
                return NotFound(new { mensaje = "No se pudo dar de baja. El chofer no existe o ya está inactivo." });
            }

            return Ok(new { mensaje = "Chofer dado de baja exitosamente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al intentar dar de baja al chofer", detalle = ex.Message });
        }
    }

    private async Task NotificarEstadoAsync(Viaje viaje, string estado, string mensaje)
    {
        var estadoDto = new EstadoViajeActualizadoDTO
        {
            IdViaje = viaje.Id,
            Estado = estado,
            Mensaje = mensaje
        };

        await _hubContext.Clients.Group($"viaje_{viaje.Id}").SendAsync("EstadoViajeActualizado", estadoDto);

        if (viaje.IdUsuario > 0)
        {
            await _hubContext.Clients.Group($"prestador_{viaje.IdUsuario}").SendAsync("EstadoViajeActualizado", estadoDto);
        }

        if (viaje.IdPasajero > 0)
        {
            await _hubContext.Clients.Group($"pasajero_{viaje.IdPasajero}").SendAsync("EstadoViajeActualizado", estadoDto);
        }

        if (viaje.IdChofer > 0)
        {
            await _hubContext.Clients.Group($"chofer_{viaje.IdChofer}").SendAsync("EstadoViajeActualizado", estadoDto);
        }
    }

    private static ViajeDetalladoDTO CrearDetalleDTO(Viaje viaje)
    {
        return new ViajeDetalladoDTO
        {
            Id = viaje.Id,
            Origen = viaje.Origen,
            Destino = viaje.Destino,
            TipoSolicitud = viaje.TipoSolicitud,
            Estado = viaje.Estado,
            EstadoPago = viaje.EstadoPago,
            Fecha = viaje.Fecha,
            Hora = viaje.Hora,
            FechaHoraReserva = viaje.FechaHoraReserva,
            Monto = viaje.Monto,
            MontoEstimado = viaje.MontoEstimado,
            MetodoDePago = viaje.MetodoDePago,
            EquipajeCarga = viaje.EquipajeCarga,
            DetalleCarga = viaje.DetalleCarga,
            MotivoCancelacion = viaje.MotivoCancelacion,
            IdPasajero = viaje.IdPasajero,
            NombrePasajero = viaje.Pasajero == null
                ? string.Empty
                : NombreCompleto(viaje.Pasajero.Nombre, viaje.Pasajero.Apellido),
            IdUsuario = viaje.IdUsuario,
            NombrePrestador = viaje.Usuario == null
                ? null
                : NombreCompleto(viaje.Usuario.Nombre, viaje.Usuario.Apellido, viaje.Usuario.RazonSocial),
            CalificadoPorPasajero = viaje.CalificadoPorPasajero,
            IdChofer = viaje.IdChofer > 0 ? viaje.IdChofer : null,
            NombreChofer = viaje.Chofer == null
                ? null
                : NombreCompleto(viaje.Chofer.Nombre, viaje.Chofer.Apellido),
            IdVehiculo = viaje.IdVehiculo > 0 ? viaje.IdVehiculo : null,
            Patente = viaje.Vehiculo?.Patente,
            Marca = viaje.Vehiculo?.Marca
        };
    }

    private static string NombreCompleto(string? nombre, string? apellido, string? razonSocial = null)
    {
        if (!string.IsNullOrWhiteSpace(razonSocial))
        {
            return razonSocial.Trim();
        }

        return $"{nombre} {apellido}".Trim();
    }
}