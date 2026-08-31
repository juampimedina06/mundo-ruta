using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundoRuta.BD.Migrations
{
    /// <inheritdoc />
    public partial class ReemplazarPrestadorPorUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Prestadores_PrestadorId",
                table: "Calificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Choferes_Prestadores_PrestadorId",
                table: "Choferes");

            migrationBuilder.DropForeignKey(
                name: "FK_Liquidaciones_Prestadores_PrestadorId",
                table: "Liquidaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Prestadores_PrestadorId",
                table: "Mensajes");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Usuarios_UsuarioId",
                table: "Mensajes");

            migrationBuilder.DropForeignKey(
                name: "FK_PrestadorServicios_Prestadores_PrestadorId",
                table: "PrestadorServicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculos_Prestadores_PrestadorId",
                table: "Vehiculos");

            migrationBuilder.DropForeignKey(
                name: "FK_Viajes_Prestadores_IdPrestador",
                table: "Viajes");

            migrationBuilder.DropTable(
                name: "Prestadores");

            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropIndex(
                name: "IX_PrestadorServicios_PrestadorId",
                table: "PrestadorServicios");

            migrationBuilder.DropIndex(
                name: "IX_Liquidaciones_PrestadorId",
                table: "Liquidaciones");

            migrationBuilder.RenameColumn(
                name: "IdPrestador",
                table: "Viajes",
                newName: "IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Viajes_IdPrestador",
                table: "Viajes",
                newName: "IX_Viajes_IdUsuario");

            migrationBuilder.RenameColumn(
                name: "PrestadorId",
                table: "Vehiculos",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "IdPrestador",
                table: "Vehiculos",
                newName: "IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Vehiculos_PrestadorId",
                table: "Vehiculos",
                newName: "IX_Vehiculos_UsuarioId");

            migrationBuilder.RenameColumn(
                name: "idPrestador",
                table: "PrestadorServicios",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "PrestadorId",
                table: "PrestadorServicios",
                newName: "IdUsuario");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Mensajes",
                newName: "EmisorId");

            migrationBuilder.RenameColumn(
                name: "PrestadorId",
                table: "Mensajes",
                newName: "DestinatarioId");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "Mensajes",
                newName: "IdEmisor");

            migrationBuilder.RenameColumn(
                name: "IdPrestador",
                table: "Mensajes",
                newName: "IdDestinatario");

            migrationBuilder.RenameIndex(
                name: "IX_Mensajes_UsuarioId",
                table: "Mensajes",
                newName: "IX_Mensajes_EmisorId");

            migrationBuilder.RenameIndex(
                name: "IX_Mensajes_PrestadorId",
                table: "Mensajes",
                newName: "IX_Mensajes_DestinatarioId");

            migrationBuilder.RenameColumn(
                name: "idPrestador",
                table: "Liquidaciones",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "PrestadorId",
                table: "Liquidaciones",
                newName: "IdUsuario");

            migrationBuilder.RenameColumn(
                name: "PrestadorId",
                table: "Choferes",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "IdPrestador",
                table: "Choferes",
                newName: "IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Choferes_PrestadorId",
                table: "Choferes",
                newName: "IX_Choferes_UsuarioId");

            migrationBuilder.RenameColumn(
                name: "PrestadorId",
                table: "Calificaciones",
                newName: "UsuarioPrestadorId");

            migrationBuilder.RenameColumn(
                name: "IdPrestador",
                table: "Calificaciones",
                newName: "IdUsuarioPrestador");

            migrationBuilder.RenameIndex(
                name: "IX_Calificaciones_PrestadorId",
                table: "Calificaciones",
                newName: "IX_Calificaciones_UsuarioPrestadorId");

            migrationBuilder.CreateIndex(
                name: "IX_PrestadorServicios_UsuarioId",
                table: "PrestadorServicios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Liquidaciones_UsuarioId",
                table: "Liquidaciones",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Usuarios_UsuarioPrestadorId",
                table: "Calificaciones",
                column: "UsuarioPrestadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Choferes_Usuarios_UsuarioId",
                table: "Choferes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Liquidaciones_Usuarios_UsuarioId",
                table: "Liquidaciones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Usuarios_DestinatarioId",
                table: "Mensajes",
                column: "DestinatarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Usuarios_EmisorId",
                table: "Mensajes",
                column: "EmisorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrestadorServicios_Usuarios_UsuarioId",
                table: "PrestadorServicios",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculos_Usuarios_UsuarioId",
                table: "Vehiculos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Viajes_Usuarios_IdUsuario",
                table: "Viajes",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Usuarios_UsuarioPrestadorId",
                table: "Calificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Choferes_Usuarios_UsuarioId",
                table: "Choferes");

            migrationBuilder.DropForeignKey(
                name: "FK_Liquidaciones_Usuarios_UsuarioId",
                table: "Liquidaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Usuarios_DestinatarioId",
                table: "Mensajes");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Usuarios_EmisorId",
                table: "Mensajes");

            migrationBuilder.DropForeignKey(
                name: "FK_PrestadorServicios_Usuarios_UsuarioId",
                table: "PrestadorServicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculos_Usuarios_UsuarioId",
                table: "Vehiculos");

            migrationBuilder.DropForeignKey(
                name: "FK_Viajes_Usuarios_IdUsuario",
                table: "Viajes");

            migrationBuilder.DropIndex(
                name: "IX_PrestadorServicios_UsuarioId",
                table: "PrestadorServicios");

            migrationBuilder.DropIndex(
                name: "IX_Liquidaciones_UsuarioId",
                table: "Liquidaciones");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "Viajes",
                newName: "IdPrestador");

            migrationBuilder.RenameIndex(
                name: "IX_Viajes_IdUsuario",
                table: "Viajes",
                newName: "IX_Viajes_IdPrestador");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Vehiculos",
                newName: "PrestadorId");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "Vehiculos",
                newName: "IdPrestador");

            migrationBuilder.RenameIndex(
                name: "IX_Vehiculos_UsuarioId",
                table: "Vehiculos",
                newName: "IX_Vehiculos_PrestadorId");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "PrestadorServicios",
                newName: "idPrestador");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "PrestadorServicios",
                newName: "PrestadorId");

            migrationBuilder.RenameColumn(
                name: "IdEmisor",
                table: "Mensajes",
                newName: "IdUsuario");

            migrationBuilder.RenameColumn(
                name: "IdDestinatario",
                table: "Mensajes",
                newName: "IdPrestador");

            migrationBuilder.RenameColumn(
                name: "EmisorId",
                table: "Mensajes",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "DestinatarioId",
                table: "Mensajes",
                newName: "PrestadorId");

            migrationBuilder.RenameIndex(
                name: "IX_Mensajes_EmisorId",
                table: "Mensajes",
                newName: "IX_Mensajes_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Mensajes_DestinatarioId",
                table: "Mensajes",
                newName: "IX_Mensajes_PrestadorId");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Liquidaciones",
                newName: "idPrestador");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "Liquidaciones",
                newName: "PrestadorId");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Choferes",
                newName: "PrestadorId");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "Choferes",
                newName: "IdPrestador");

            migrationBuilder.RenameIndex(
                name: "IX_Choferes_UsuarioId",
                table: "Choferes",
                newName: "IX_Choferes_PrestadorId");

            migrationBuilder.RenameColumn(
                name: "UsuarioPrestadorId",
                table: "Calificaciones",
                newName: "PrestadorId");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioPrestador",
                table: "Calificaciones",
                newName: "IdPrestador");

            migrationBuilder.RenameIndex(
                name: "IX_Calificaciones_UsuarioPrestadorId",
                table: "Calificaciones",
                newName: "IX_Calificaciones_PrestadorId");

            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prestadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdministradorId = table.Column<int>(type: "int", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cbu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComisionPorcentaje = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cuit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdAdministrador = table.Column<int>(type: "int", nullable: false),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NombreComercial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Provincia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RazonSocial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoPersona = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prestadores_Administradores_AdministradorId",
                        column: x => x.AdministradorId,
                        principalTable: "Administradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrestadorServicios_PrestadorId",
                table: "PrestadorServicios",
                column: "PrestadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Liquidaciones_PrestadorId",
                table: "Liquidaciones",
                column: "PrestadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestadores_AdministradorId",
                table: "Prestadores",
                column: "AdministradorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Prestadores_PrestadorId",
                table: "Calificaciones",
                column: "PrestadorId",
                principalTable: "Prestadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Choferes_Prestadores_PrestadorId",
                table: "Choferes",
                column: "PrestadorId",
                principalTable: "Prestadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Liquidaciones_Prestadores_PrestadorId",
                table: "Liquidaciones",
                column: "PrestadorId",
                principalTable: "Prestadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Prestadores_PrestadorId",
                table: "Mensajes",
                column: "PrestadorId",
                principalTable: "Prestadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Usuarios_UsuarioId",
                table: "Mensajes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrestadorServicios_Prestadores_PrestadorId",
                table: "PrestadorServicios",
                column: "PrestadorId",
                principalTable: "Prestadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculos_Prestadores_PrestadorId",
                table: "Vehiculos",
                column: "PrestadorId",
                principalTable: "Prestadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Viajes_Prestadores_IdPrestador",
                table: "Viajes",
                column: "IdPrestador",
                principalTable: "Prestadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
