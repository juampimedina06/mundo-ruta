using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.Repositorio.Repositorios;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/servicios")]
    public class ServiciosController : ControllerBase
    {
        private readonly IUsuarioRepositorio repo;

        public ServiciosController(IUsuarioRepositorio usuarioRepo)
        {
            repo = usuarioRepo;
        }

        // GET 
        // Muestra el listado de todos los Prestadores disponibles para contratar.
        [HttpGet("fletes")] // api/servicios/fletes
        public async Task<ActionResult<List<PrestadorListadoDTO>>> GetFletes()
        {
            var prestadores = await repo.ObtenerPrestadoresPorEstado("APROBADO");
            return Ok(prestadores);
        }
    }
}