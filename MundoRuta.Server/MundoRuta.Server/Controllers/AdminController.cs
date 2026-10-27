using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Repositorio.Repositorios;
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
        private readonly AppDbContext _context;

        public AdminController(IUsuarioRepositorio usuarioRepositorio, IViajeRepositorio viajeRepositorio, AppDbContext context)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _viajeRepositorio = viajeRepositorio;
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

        [HttpGet("dashboard")] // api/admin/dashboard
        public async Task<IActionResult> GetDashboard()
        {
            var totalUsuarios = await _usuarioRepositorio.ContarPorRolYEstado("Usuario", null);
            var totalPrestadores = await _usuarioRepositorio.ContarPorRolYEstado("Prestador", "APROBADO");
            var viajesConcretados = await _viajeRepositorio.ContarViajesFinalizados();

            return Ok(new
            {
                totalUsuarios,
                totalPrestadores,
                viajesConcretados
            });
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
    }
}