using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MundoRuta.Server.Client.Shared.Constants
{
    public static class Roles
    {
        public const string Usuario = "Usuario";
        public const string Pasajero = "Usuario"; // Alias por compatibilidad con el ticket
        public const string Prestador = "Prestador";
        public const string Chofer = "Chofer";
        public const string Admin = "Admin";
    }
}
