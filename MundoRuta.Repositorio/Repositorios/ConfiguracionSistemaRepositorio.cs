using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;

namespace MundoRuta.Repositorio.Repositorios
{
    public class ConfiguracionSistemaRepositorio : Repositorio<ConfiguracionSistema>, IConfiguracionSistemaRepositorio
    {
        public ConfiguracionSistemaRepositorio(AppDbContext context) : base(context)
        {
        }
    }
}
