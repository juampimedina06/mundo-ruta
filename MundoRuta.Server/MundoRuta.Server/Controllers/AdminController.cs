using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Repositorio.Repositorios;
using MundoRuta.Shared;
using MundoRuta.Shared.DTO;
using System.Globalization;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IViajeRepositorio _viajeRepositorio;
        private readonly ILiquidacionRepositorio _liquidacionRepositorio;
        private readonly ICalificacionRepositorio _calificacionRepositorio;
        private readonly IChoferRepositorio _choferRepositorio;
        private readonly IVehiculoRepositorio _vehiculoRepositorio;
        private readonly AppDbContext _context;

        public AdminController(IUsuarioRepositorio usuarioRepositorio, IViajeRepositorio viajeRepositorio, ILiquidacionRepositorio liquidacionRepositorio, ICalificacionRepositorio calificacionRepositorio, IChoferRepositorio choferRepositorio, IVehiculoRepositorio vehiculoRepositorio, AppDbContext context)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _viajeRepositorio = viajeRepositorio;
            _liquidacionRepositorio = liquidacionRepositorio;
            _calificacionRepositorio = calificacionRepositorio;
            _choferRepositorio = choferRepositorio;
            _vehiculoRepositorio = vehiculoRepositorio;
            _context = context;
        }

        [HttpGet("prestadores-pendientes")] // api/admin/prestadores-pendientes
        public async Task<ActionResult<List<PrestadorListadoDTO>>> ListaPrestadoresPendientes()
        {
            // Pasamos un solo argumento ("PENDIENTE")
            var listaPrestadoresPendientes = await _usuarioRepositorio.ObtenerPrestadoresPorEstado("PENDIENTE");

            if (listaPrestadoresPendientes == null || !listaPrestadoresPendientes.Any())
            {
                return NotFound(new { mensaje = "No hay prestadores pendientes" });
            }

            return Ok(listaPrestadoresPendientes);
        }

        [HttpPut("aprobar-prestador/{id:int}")]
        public async Task<IActionResult> AprobarPrestador(int id)
        {
            var prestador = await _usuarioRepositorio.ObtenerPorId(id);

            if (prestador == null || prestador.Rol != "Prestador")
            {
                return NotFound(new { mensaje = "Prestador no encontrado" });
            }

            prestador.Estado = "APROBADO";
            await _usuarioRepositorio.Update(prestador);

            return Ok(new { mensaje = "Prestador aprobado correctamente" });
        }

        [HttpGet("prestadores/solicitudes")] // api/admin/prestadores/solicitudes
        public async Task<ActionResult<List<PrestadorSolicitudDTO>>> ListaSolicitudesPrestadores()
        {
            List<PrestadorSolicitudDTO> solicitudes = await _usuarioRepositorio.ListarSolicitudesPrestador();

            // Se devuelve 200 con lista vacía para que el cliente distinga "sin datos" de "error".
            return Ok(solicitudes);
        }

        [HttpPut("prestadores/{id:int}/estado")] // api/admin/prestadores/{id}/estado
        public async Task<IActionResult> ActualizarEstadoPrestador(int id, [FromBody] ActualizarEstadoPrestadorDTO dto)
        {
            if (!EstadosPrestador.EsAsignable(dto.Estado))
            {
                return BadRequest(new { mensaje = "Estado inválido. Valores permitidos: APROBADO, RECHAZADO o SUSPENDIDO." });
            }

            Usuario? prestador = await _usuarioRepositorio.ObtenerPorId(id);

            if (prestador == null || prestador.Rol != Roles.Prestador)
            {
                return NotFound(new { mensaje = "Prestador no encontrado" });
            }

            bool actualizado = await _usuarioRepositorio.CambiarEstado(id, dto.Estado);

            if (!actualizado)
            {
                return NotFound(new { mensaje = "Prestador no encontrado" });
            }

            return Ok(new { mensaje = "Estado actualizado correctamente", id, estado = dto.Estado });
        }

        [HttpGet("dashboard")] // api/admin/dashboard
        public async Task<ActionResult<AdminDashboardDTO>> GetDashboard()
        {
            int totalPasajeros = await _usuarioRepositorio.ContarPorRolYEstado("Usuario", null);
            int totalPrestadores = await _usuarioRepositorio.ContarPorRolYEstado("Prestador", null);
            int totalChoferes = await _usuarioRepositorio.ContarPorRolYEstado("Chofer", null);
            int prestadoresPendientes = await _usuarioRepositorio.ContarPorRolYEstado("Prestador", "PENDIENTE");
            int totalViajes = await _viajeRepositorio.ContarTodos();
            int viajesActivos = await _viajeRepositorio.ContarActivos();
            decimal volumenTransaccionado = await _viajeRepositorio.SumarMontoTransaccionado();
            decimal comisiones = await _liquidacionRepositorio.SumarComisiones();

            AdminDashboardDTO dto = new AdminDashboardDTO
            {
                TotalPasajeros = totalPasajeros,
                TotalPrestadores = totalPrestadores,
                TotalChoferes = totalChoferes,
                TotalUsuarios = totalPasajeros + totalPrestadores + totalChoferes,
                PrestadoresPendientes = prestadoresPendientes,
                TotalViajes = totalViajes,
                ViajesActivos = viajesActivos,
                VolumenTransaccionado = volumenTransaccionado,
                Comisiones = comisiones,
                FechaCorte = DateTime.UtcNow
            };

            return Ok(dto);
        }

        [HttpGet("usuarios")] // api/admin/usuarios
        public async Task<ActionResult<List<AdminUsuarioListadoDTO>>> ListarUsuarios()
        {
            List<AdminUsuarioListadoDTO> usuarios = await _usuarioRepositorio.ListarUsuarios();

            // Se devuelve 200 con lista vacía para que el cliente distinga "sin datos" de "error".
            return Ok(usuarios);
        }

        [HttpGet("usuarios/{id:int}")] // api/admin/usuarios/{id}
        public async Task<ActionResult<AdminUsuarioDetalleDTO>> ObtenerUsuario(int id)
        {
            Usuario? usuario = await _usuarioRepositorio.ObtenerPorId(id);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            (double? Promedio, int Total) recibidas = await _calificacionRepositorio.ObtenerPromedioRecibido(id);
            (double? Promedio, int Total) otorgadas = await _calificacionRepositorio.ObtenerPromedioOtorgado(id);

            AdminUsuarioDetalleDTO detalle = new AdminUsuarioDetalleDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                Rol = usuario.Rol,
                Estado = usuario.Estado,
                // FechaRegistro se guarda en UTC y SQL Server la devuelve con Kind = Unspecified.
                FechaRegistro = DateTime.SpecifyKind(usuario.FechaRegistro, DateTimeKind.Utc),
                RazonSocial = usuario.RazonSocial,
                Cuit = usuario.Cuit,
                TarifaBaseKm = usuario.TarifaBaseKm,
                PromedioRecibido = recibidas.Promedio,
                TotalRecibidas = recibidas.Total,
                PromedioOtorgado = otorgadas.Promedio,
                TotalOtorgadas = otorgadas.Total
            };

            if (usuario.Rol == Roles.Prestador)
            {
                detalle.Vehiculos = await _vehiculoRepositorio.ObtenerPorPrestador(id);
                detalle.Choferes = await _choferRepositorio.ObtenerPorPrestador(id);
                detalle.CantidadViajes = await _viajeRepositorio.ContarViajesDeUsuario(id);
            }
            else if (usuario.Rol == Roles.Chofer)
            {
                Chofer? chofer = await _choferRepositorio.ObtenerPorUsuario(id);
                detalle.CantidadViajes = chofer == null ? 0 : await _viajeRepositorio.ContarViajesDeChofer(chofer.Id);
            }
            else
            {
                detalle.CantidadViajes = await _viajeRepositorio.ContarViajesDeUsuario(id);
            }

            return Ok(detalle);
        }

        [HttpGet("configuracion/recargo-acompanante")]
        public async Task<IActionResult> GetRecargoAcompanante()
        {
            var config = await _context.ConfiguracionesSistema.FirstOrDefaultAsync(c => c.Clave == "RECARGO_ACOMPANANTE_PORCENTAJE");
            decimal valor = 15m;
            if (config != null)
            {
                if (decimal.TryParse(config.Valor.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorInv))
                {
                    valor = valorInv;
                }
                else if (decimal.TryParse(config.Valor, out decimal valorLoc))
                {
                    valor = valorLoc;
                }
            }
            return Ok(new { porcentajeRecargoAcompanante = valor });
        }

        [HttpPut("configuracion/recargo-acompanante")]
        public async Task<IActionResult> SetRecargoAcompanante(ConfiguracionRecargoDTO dto)
        {
            var config = await _context.ConfiguracionesSistema.FirstOrDefaultAsync(c => c.Clave == "RECARGO_ACOMPANANTE_PORCENTAJE");
            if (config == null)
            {
                config = new ConfiguracionSistema
                {
                    Clave = "RECARGO_ACOMPANANTE_PORCENTAJE",
                    Valor = dto.PorcentajeRecargoAcompanante.ToString(CultureInfo.InvariantCulture),
                    Descripcion = "Porcentaje de recargo por acompañante"
                };
                await _context.ConfiguracionesSistema.AddAsync(config);
            }
            else
            {
                config.Valor = dto.PorcentajeRecargoAcompanante.ToString(CultureInfo.InvariantCulture);
            }
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "RECARGO POR ACOMPAÑANTE ACTUALIZADO", porcentajeRecargoAcompanante = dto.PorcentajeRecargoAcompanante });
        }

        [HttpGet("comisiones")] // api/admin/comisiones
        public async Task<ActionResult<AdminComisionesReporteDTO>> ObtenerComisiones([FromQuery] AdminFiltroComisionesDTO filtro)
        {
            decimal porcentajeComision = 10m;
            try
            {
                ConfiguracionSistema? config = await _context.ConfiguracionesSistema
                    .FirstOrDefaultAsync(c => c.Clave == "COMISION_PLATAFORMA_PORCENTAJE");

                if (config != null)
                {
                    if (decimal.TryParse(config.Valor.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorInv))
                    {
                        porcentajeComision = valorInv;
                    }
                    else if (decimal.TryParse(config.Valor, out decimal valorLoc))
                    {
                        porcentajeComision = valorLoc;
                    }
                }
            }
            catch (Exception)
            {
                // Fallback por defecto en caso de que la tabla de configuración no esté creada en la base local.
            }

            AdminComisionesReporteDTO reporte = await _viajeRepositorio.ObtenerReporteComisiones(filtro, porcentajeComision);
            return Ok(reporte);
        }
    }
}
