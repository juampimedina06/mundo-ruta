using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundoRuta.BD.Migrations
{
    /// <inheritdoc />
    public partial class ArreglarFkCalificaciones2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Usuarios_UsuarioId",
                table: "Calificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Viajes_ViajeId",
                table: "Calificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_UsuarioId",
                table: "Calificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_ViajeId",
                table: "Calificaciones");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Calificaciones");

            migrationBuilder.DropColumn(
                name: "ViajeId",
                table: "Calificaciones");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_IdUsuario",
                table: "Calificaciones",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_IdViaje",
                table: "Calificaciones",
                column: "IdViaje");

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Usuarios_IdUsuario",
                table: "Calificaciones",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Viajes_IdViaje",
                table: "Calificaciones",
                column: "IdViaje",
                principalTable: "Viajes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Usuarios_IdUsuario",
                table: "Calificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Viajes_IdViaje",
                table: "Calificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_IdUsuario",
                table: "Calificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_IdViaje",
                table: "Calificaciones");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Calificaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ViajeId",
                table: "Calificaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_UsuarioId",
                table: "Calificaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_ViajeId",
                table: "Calificaciones",
                column: "ViajeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Usuarios_UsuarioId",
                table: "Calificaciones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Viajes_ViajeId",
                table: "Calificaciones",
                column: "ViajeId",
                principalTable: "Viajes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
