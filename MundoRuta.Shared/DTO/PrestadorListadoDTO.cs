using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO;

public class PrestadorListadoDTO
{
    public int Id { get; set; }
    public string RazonSocial { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }
    public string Ciudad { get; set; }
}
