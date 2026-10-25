using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MundoRuta.BD.Datos.Entity;

public class Viaje : EntidadBase
{
    public required string Origen { get; set; }

    public required string Destino { get; set; }

    public required string TipoSolicitud { get; set; }

    public string Estado { get; set; }

    public string EstadoPago { get; set; }

    public DateTime Fecha { get; set; }

    public TimeSpan Hora { get; set; }

    public bool EquipajeCarga { get; set; }

    // Características y condiciones del servicio de flete
    public bool EsCubierto { get; set; }

    public bool UsaBaulera { get; set; }

    public bool LlevaAcompanante { get; set; }

    // DNI de la persona que recibe la carga.
    public string DniReceptor { get; set; } 

    // Indica si cada parte calificó el viaje.
    public bool CalificadoPorPasajero { get; set; } = false;

    public bool CalificadoPorChofer { get; set; } = false;



    public decimal Monto { get; set; }

    public string MetodoDePago { get; set; }
    public decimal MontoEstimado { get; set; }

    public int IdSolicitante { get; set; }
    
    public Usuario Solicitante { get; set; }

    public int IdPasajero { get; set; }

    public Usuario Pasajero { get; set; }

    public int IdUsuario { get; set; }

    public Usuario Usuario { get; set; }

    public int IdChofer { get; set; }

    public Chofer Chofer { get; set; }

    public int IdServicio { get; set; }

    public Servicio Servicio { get; set; }

    public DateTime FechaHoraReserva { get; set; } //

    public string DetalleCarga { get; set; } //

    public string? MotivoCancelacion { get; set; } //

    public int IdVehiculo { get; set; }

    [ForeignKey("IdVehiculo")]
    public Vehiculo Vehiculo { get; set; }






}
