using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Repositorio;
using MundoRuta.Repositorio.Repositorios;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMensajeRepositorio _mensajeRepo;
        private readonly IChatRepositorio _chatRepo; 

        public ChatController(AppDbContext context, IMensajeRepositorio mensajeRepo, IChatRepositorio chatRepo)
        {
            _context = context;
            _mensajeRepo = mensajeRepo;
            _chatRepo = chatRepo;
        }

        [HttpPost("viajes/{viajeId}/mensajes")]
        public async Task<ActionResult> EnviarMensaje(int viajeId, [FromBody] CrearMensajeDTO dto)
        {
            var viaje = await _context.Viajes.FindAsync(viajeId);
            if (viaje == null) return NotFound("Viaje no encontrado.");


            var estadosActivos = new[] { "Asignado", "Carga-subida", "En-transito", "Llegada-destino" };
            if (!estadosActivos.Contains(viaje.Estado))
            {
                return BadRequest(new { mensaje = "El chat está deshabilitado. El viaje no se encuentra en curso." });
            }


            var chat = await _chatRepo.ObtenerPorViajeId(viajeId);
            if (chat == null)
            {
                var nuevoChat = new Chat { IdViaje = viaje, FechaInicio = DateTime.Now };
                await _chatRepo.CrearChat(nuevoChat);
                chat = nuevoChat;
            }

            int idReceptor = dto.IdEmisor == viaje.IdUsuario
                            ? viaje.IdChofer
                            : viaje.IdUsuario;


            var mensaje = new Mensaje
            {
                IdChat = chat.Id,
                IdEmisor = dto.IdEmisor,
                IdDestinatario   = idReceptor,
                Contenido = dto.Contenido,
                EnviadoEn = DateTime.Now
            };

            await _mensajeRepo. CrearMensaje(mensaje); 

            return Ok(new { mensaje = "Mensaje enviado correctamente." });
        }

        [HttpGet("viajes/{viajeId}/mensajes")]
        public async Task<ActionResult> ObtenerMensajes(int viajeId)
        {
            var viaje = await _context.Viajes.FindAsync(viajeId);
            if (viaje == null) return NotFound();

            var estadosActivos = new[] { "Asignado", "Carga-subida", "En-transito", "Llegada-destino" };
            if (!estadosActivos.Contains(viaje.Estado))
            {
                return BadRequest("No se pueden leer los mensajes de un viaje inactivo o cerrado.");
            }

            var chat = await _chatRepo.ObtenerPorViajeId(viajeId);
            if (chat == null)
            {
                return Ok(new List<MensajeDTO>()); 
            }
            var mensajesEntidad = await _mensajeRepo.ObtenerPorChatId(chat.Id);

 
            var mensajesDTO = mensajesEntidad.Select(m => new MensajeDTO
            {
                Id = m.Id,
                IdEmisor = m.IdEmisor,
                IdDestinatario   = m.IdDestinatario,
                Contenido = m.Contenido,
                EnviadoEn = m.EnviadoEn
            }).ToList();

            return Ok(mensajesDTO);
        }
    }
}