# MundoRuta
# 🏗️ Estructura Full Stack con Blazor y C#

Guía paso a paso para configurar una solución .NET con arquitectura en capas usando Blazor WebAssembly, Entity Framework Core y SQL Server.

---

## 📁 Estructura de la Solución

```
NombreSolucion/
├── Nombre.Server/          # Proyecto principal Blazor (host + API)
├── Nombre.Client/          # Proyecto Blazor WebAssembly (frontend)
├── Nombre.Shared/          # Biblioteca de clases compartidas (DTOs, modelos)
├── Nombre.Servicios/       # Biblioteca de clases (lógica de servicios)
├── Nombre.BD/              # Biblioteca de clases (contexto de base de datos)
│   └── Datos/
│       └── Entity/         # Entidades de la base de datos
└── Nombre.Repositorio/     # Biblioteca de clases (acceso a datos)
```

---

## 🔗 Diagrama de Dependencias

```
Nombre.Shared
    ├── ← Nombre.BD
    │       └── ← Nombre.Repositorio
    ├── ← Nombre.Client
    │       └── ← Nombre.Servicios
    └── ← Nombre.Server
            ├── ← Nombre.Client
            ├── ← Nombre.Shared
            ├── ← Nombre.BD
            └── ← Nombre.Repositorio
```

| Proyecto | Depende de |
|---|---|
| `Nombre.BD` | `Nombre.Shared` |
| `Nombre.Repositorio` | `Nombre.BD`, `Nombre.Shared` |
| `Nombre.Client` | `Nombre.Shared`, `Nombre.Servicios` |
| `Nombre.Server` | `Nombre.Client`, `Nombre.Shared`, `Nombre.BD`, `Nombre.Repositorio` |

---

## 🚀 Paso a Paso

### 1. Crear el Proyecto Blazor (Server)

En Visual Studio:

1. **Nuevo proyecto** → seleccionar **Blazor Web App**
2. Nombre: `Nombre.Server`
3. Al crearlo, Visual Studio genera automáticamente el proyecto complementario `Nombre.Client`

---

### 2. Crear `Nombre.Shared`

1. Click derecho en la solución → **Agregar** → **Nuevo Proyecto**
2. Seleccionar **Biblioteca de clases (.NET)**
3. Nombre: `Nombre.Shared`

**Agregar referencia en `Nombre.Client`:**
> Click derecho en `Nombre.Client` → **Agregar** → **Referencia de proyecto** → seleccionar `Nombre.Shared`

**Agregar referencia en `Nombre.Server`:**
> Click derecho en `Nombre.Server` → **Agregar** → **Referencia de proyecto** → seleccionar `Nombre.Shared`

---

### 3. Crear `Nombre.Servicios`

1. Agregar nueva **Biblioteca de clases** → Nombre: `Nombre.Servicios`

**Agregar referencia en `Nombre.Client`:**
> Click derecho en `Nombre.Client` → **Agregar** → **Referencia de proyecto** → seleccionar `Nombre.Servicios`

---

### 4. Crear `Nombre.BD`

1. Agregar nueva **Biblioteca de clases** → Nombre: `Nombre.BD`

**Agregar referencia en `Nombre.BD`:**
> Click derecho en `Nombre.BD` → **Agregar** → **Referencia de proyecto** → seleccionar `Nombre.Shared`

**Estructura de carpetas dentro de `Nombre.BD`:**

```
Nombre.BD/
└── Datos/
    └── Entity/   ← aquí van las entidades
```

---

### 5. Crear `Nombre.Repositorio`

1. Agregar nueva **Biblioteca de clases** → Nombre: `Nombre.Repositorio`

**Agregar referencias en `Nombre.Repositorio`:**
> Click derecho → **Agregar** → **Referencia de proyecto** → seleccionar `Nombre.BD` y `Nombre.Shared`

---

### 6. Agregar referencias finales en `Nombre.Server`

> Click derecho en `Nombre.Server` → **Agregar** → **Referencia de proyecto**

Seleccionar todos:
- `Nombre.Client`
- `Nombre.Shared`
- `Nombre.BD`
- `Nombre.Repositorio`

---

## 📦 Paquetes NuGet

> Para instalar: click derecho en el proyecto → **Administrar paquetes NuGet** → buscar e instalar

### En `Nombre.BD`:

| Paquete | Descripción |
|---|---|
| `Microsoft.EntityFrameworkCore.SqlServer` | Proveedor de SQL Server para EF Core |
| `Microsoft.EntityFrameworkCore.Tools` | Herramientas para migraciones (`add-migration`, `update-database`) |

### En `Nombre.Server`:

| Paquete | Descripción |
|---|---|
| `Microsoft.EntityFrameworkCore.SqlServer` | Necesario para el registro del contexto |
| `Microsoft.EntityFrameworkCore.Tools` | Herramientas de EF Core |

---

## 🗃️ Configurar `AppDbContext`

Dentro de `Nombre.BD/Datos/`, crear la clase `AppDbContext`:

```csharp
using Microsoft.EntityFrameworkCore;
using Nombre.BD.Datos.Entity;

public class AppDbContext : DbContext
{
    public DbSet<NombreEntidad> NombreEntidades { get; set; }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
}
```

> Reemplazar `NombreEntidad` y `NombreEntidades` con el nombre real de tu entidad.

---

## ⚙️ Configurar la Cadena de Conexión

### `appsettings.json` (en `Nombre.Server`)

```json
{
  "ConnectionStrings": {
    "ConnSqlServer": "url base de datos"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

> Reemplazar `"url base de datos"` con tu cadena de conexión real. Ejemplo:
> ```
> Server=localhost;Database=MiBaseDeDatos;Trusted_Connection=True;TrustServerCertificate=True;
> ```

### `Program.cs` (en `Nombre.Server`)

Agregar debajo de `var builder = WebApplication.CreateBuilder(args);`:

```csharp
string connectionString = builder.Configuration.GetConnectionString("ConnSqlServer")
    ?? throw new InvalidOperationException("No existe la conexión con la base de datos.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
```

---

## 🧱 Estructura de una Entidad (ejemplo)

Dentro de `Nombre.BD/Datos/Entity/`:

```csharp
public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}
```

Luego registrar en `AppDbContext`:

```csharp
public DbSet<Producto> Productos { get; set; }
```

---

## 🔄 Migraciones

Una vez configurado el contexto y la cadena de conexión, ejecutar desde la **Consola del Administrador de Paquetes** (seleccionar `Nombre.Server` como proyecto de inicio):

```powershell
# Crear migración inicial
Add-Migration Inicial -Project Nombre.BD

# Aplicar a la base de datos
Update-Database
```

---

## ✅ Checklist de Configuración

- [ ] Proyecto `Nombre.Server` creado (Blazor Web App)
- [ ] Proyecto `Nombre.Client` generado automáticamente
- [ ] `Nombre.Shared` creado y referenciado en `Client` y `Server`
- [ ] `Nombre.Servicios` creado y referenciado en `Client`
- [ ] `Nombre.BD` creado con carpeta `Datos/Entity/`
- [ ] `Nombre.BD` referencia a `Shared`
- [ ] `Nombre.Repositorio` creado y referencia a `BD` y `Shared`
- [ ] `Nombre.Server` referencia a `Client`, `Shared`, `BD` y `Repositorio`
- [ ] NuGet `EntityFrameworkCore.SqlServer` instalado en `BD` y `Server`
- [ ] NuGet `EntityFrameworkCore.Tools` instalado en `BD` y `Server`
- [ ] `AppDbContext` creado en `Nombre.BD/Datos/`
- [ ] Cadena de conexión configurada en `appsettings.json`
- [ ] Registro del contexto en `Program.cs`
- [ ] Migraciones creadas y aplicadas
