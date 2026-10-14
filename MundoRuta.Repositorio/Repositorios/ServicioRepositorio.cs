using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Repositorio.Repositorios;

public class ServicioRepositorio : Repositorio<Servicio>, IServicioRepositorio
{
    private readonly AppDbContext context;
    public ServicioRepositorio(AppDbContext context) : base(context)
    {
        this.context = context;
    }

    public async Task<Servicio> ObtenerOCrearPorDefecto()
    {
        var servicio = await context.Servicios.FirstOrDefaultAsync();
        if (servicio != null) return servicio;

        servicio = new Servicio { Nombre = "Flete", Descripcion = "Flete general", Tipo = "F", Estado = "ACTIVO" };
        context.Servicios.Add(servicio);
        await context.SaveChangesAsync();
        return servicio;
    }

}
