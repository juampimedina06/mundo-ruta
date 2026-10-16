using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundoRuta.BD.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFKPrestadorNotificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_PrestadorId",
                table: "Notificaciones",
                column: "PrestadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_PrestadorServicios_PrestadorId",
                table: "Notificaciones",
                column: "PrestadorId",
                principalTable: "PrestadorServicios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_PrestadorServicios_PrestadorId",
                table: "Notificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Notificaciones_PrestadorId",
                table: "Notificaciones");
        }
    }
}
