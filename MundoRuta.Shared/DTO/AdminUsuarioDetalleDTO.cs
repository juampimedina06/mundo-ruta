namespace MundoRuta.Shared.DTO
{
    public class AdminUsuarioDetalleDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
        public decimal? TarifaBaseKm { get; set; }
        public double? PromedioRecibido { get; set; }
        public int TotalRecibidas { get; set; }
        public double? PromedioOtorgado { get; set; }
        public int TotalOtorgadas { get; set; }
        public int CantidadViajes { get; set; }
        public List<VehiculoDTO> Vehiculos { get; set; } = new List<VehiculoDTO>();
        public List<ChoferDTO> Choferes { get; set; } = new List<ChoferDTO>();
    }
}
