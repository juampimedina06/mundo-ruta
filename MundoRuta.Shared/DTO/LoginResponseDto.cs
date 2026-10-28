namespace MundoRuta.Shared.DTO
{
    public class LoginResponseDto
    {
        public string Mensaje { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
