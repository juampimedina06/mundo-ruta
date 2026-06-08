using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity;

public class Viaje : EntidadBase
{
    public required string Origen { get; set; }

    public required string Destino { get; set; }

    public required string TipoSolicitud { get; set; }

    public required Boolean Estado { get; set; }

    public DateTime Fecha { get; set; }

    public TimeSpan Hora { get; set; }

    public bool EquipajeCarga { get; set; }

    public decimal Monto { get; set; }

    public int IdSolicitante { get; set; }
    
    public Usuario Solicitante { get; set; }

    public int IdPasajero { get; set; }

    public Usuario Pasajero { get; set; }

    public int IdPrestador { get; set; }

    public Prestador Prestador { get; set; }

    public int IdChofer { get; set; }

    public Chofer Chofer { get; set; }

    public int IdServicio { get; set; }

    public Servicio Servicio { get; set; }

    




}
