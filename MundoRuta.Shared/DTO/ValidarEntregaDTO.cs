using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO
{
    public class ValidarEntregaDTO
    {
        private string _dniIngresado = string.Empty;

        public string DniIngresado
        {
            get => !string.IsNullOrWhiteSpace(_dniIngresado) ? _dniIngresado : CuitIngresado;
            set => _dniIngresado = value;
        }

        public string CuitIngresado { get; set; } = string.Empty;

        public int PuntajePasajero { get; set; }

        public string? Comentario { get; set; }
    }
}
