using Microsoft.AspNetCore.Mvc;
using MundoRuta.Repositorio.Repositorios;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IViajeRepositorio _viajeRepositorio;

        public AdminController(IUsuarioRepositorio usuarioRepositorio, IViajeRepositorio viajeRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _viajeRepositorio = viajeRepositorio;
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
    }
}