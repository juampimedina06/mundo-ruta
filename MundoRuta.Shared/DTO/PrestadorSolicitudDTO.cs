namespace MundoRuta.Shared.DTO
{
    // Datos de una empresa de transporte que se registró y espera la evaluación del administrador.
    public class PrestadorSolicitudDTO
    {
        public int Id { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
