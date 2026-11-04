using System;
using System.Globalization;

namespace MundoRuta.Server.Client.Infraestructura
{
    // Da formato a las fechas y montos que se muestran en el panel de administración.
    public static class FormatoPanel
    {
        // La API envía las fechas en UTC: se convierten a la hora local antes de mostrarlas.
        // El panel exige el formato dd/MM/yyyy, por eso se ignora la cultura del navegador.
        public static string Fecha(DateTime fecha)
        {
            return fecha.ToLocalTime().ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        // Muestra el monto con separador de miles y el prefijo "$ " del peso argentino.
        public static string Moneda(decimal monto)
        {
            return "$ " + monto.ToString("N0", CultureInfo.GetCultureInfo("es-AR"));
        }
    }
}
