using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MundoRuta.Shared.DTO;

public class CalificacionesDTO //entrada. Datos que manda el cliente al calificar un viaje
{

    public int viajeId {  get; set; }
    public int usuarioId { get; set; }

    public int usuarioPrestadorId { get; set; }
    public int puntaje { get; set; }
    public string comentario { get; set; }
}

public class CalificacionesResponseDTO 
{
    public int id { get; set; }
    public int viajeId { get; set; }
    public int usuarioId { get; set; }
    public int usuarioPrestadorId { get; set; }
    public int puntaje { get; set; }
    public string comentario { get; set; }

}
public class CalificacionItemDTO
{
    public int id { get; set; }
    public int puntaje { get; set; }
    public string comentario { get; set; } = string.Empty;
    public int usuarioId { get; set; }
    public int viajeId { get; set; }
    public string nombrePasajero { get; set; } = string.Empty;
    public string origen { get; set; } = string.Empty;
    public string destino { get; set; } = string.Empty;
    public string tipoSolicitud { get; set; } = string.Empty;
    public DateTime? fechaViaje { get; set; }
    public string nombreChofer { get; set; } = string.Empty;
    public string vehiculo { get; set; } = string.Empty;
}


public class ListadoCalificacionesDTO
{
    public double promedioCalificacion { get; set; }
    public int totalReseñas { get; set; }
    public List<CalificacionItemDTO> comentarios { get; set; } = new();
}