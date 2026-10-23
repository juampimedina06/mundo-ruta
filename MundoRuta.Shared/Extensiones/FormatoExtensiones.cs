using System;
using System.Globalization;

namespace MundoRuta.Shared.Extensiones;

public static class FormatoExtensiones
{
    public const string FormatoFecha = "dd/MM/yyyy";

    public static string ToFechaCorta(this DateTime fecha)
        => fecha.ToString(FormatoFecha, CultureInfo.InvariantCulture);

    public static string ToFechaCorta(this DateTime? fecha, string valorVacio = "-")
        => fecha.HasValue ? fecha.Value.ToFechaCorta() : valorVacio;

    public static string ToHoraCorta(this TimeSpan hora)
        => hora.ToString(@"hh\:mm", CultureInfo.InvariantCulture);

    // Formato monetario es-AR (1.234,50)
    public static string ToMonto(this decimal monto)
        => "$ " + monto.ToString("N2", CultureInfo.InvariantCulture)
                       .Replace(",", "#").Replace(".", ",").Replace("#", ".");
}
