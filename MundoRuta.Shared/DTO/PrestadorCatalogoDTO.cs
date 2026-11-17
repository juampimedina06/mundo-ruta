namespace MundoRuta.Shared.DTO
{
    public class PrestadorCatalogoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public double PromedioCalificacion { get; set; }
        public int TotalCalificaciones { get; set; }
    }
}
