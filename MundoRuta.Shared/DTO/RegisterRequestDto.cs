using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO
{
    public class RegisterRequestDto
    {
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
    }
}
