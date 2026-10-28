namespace MundoRuta.Server.Client.Navigation
{
    // Representa un enlace del menú de navegación.
    public sealed class MenuItem
    {
        public MenuItem(string texto, string href)
        {
            Texto = texto;
            Href = href;
        }

        public string Texto { get; }

        public string Href { get; }
    }
}
