using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity;

public class Prestador : EntidadBase
{

    public string RazonSocial { get; set; }
    public string NombreComercial { get; set; }
    public string Cuit { get; set; }
    public string TipoPersona { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; } 
    public string Direccion { get; set; }
    public string Ciudad { get; set; }
    public string Provincia { get; set; }
    public string Estado { get; set; }
    public DateTime FechaAlta { get; set; }
    public string? MotivoRechazo { get; set; }
    public string Cbu { get; set; }
    public string? Alias { get; set; }
    public decimal ComisionPorcentaje { get; set; }
    public string? DocumentoUrl { get; set; }
    public int IdAdministrador { get; set; }
    public Administrador Administrador { get; set; }

}
