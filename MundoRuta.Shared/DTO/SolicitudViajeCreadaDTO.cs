namespace MundoRuta.Shared.DTO
{
    // Respuesta del alta de una solicitud de viaje: incluye el Id con el que se abre la subasta.
    public class SolicitudViajeCreadaDTO
    {
        public string Mensaje { get; set; } = string.Empty;
        public int Id { get; set; }
        public decimal Monto { get; set; }
    }
}
