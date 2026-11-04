using System;

namespace MundoRuta.Shared.DTO.Realtime
{
    public class NuevoMensajeChatDTO
    {
        public int IdMensaje { get; set; }
        public int IdChat { get; set; }
        public int IdEmisor { get; set; }
        public int? IdDestinatario { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public DateTime EnviadoEn { get; set; } = DateTime.Now;
    }
}
