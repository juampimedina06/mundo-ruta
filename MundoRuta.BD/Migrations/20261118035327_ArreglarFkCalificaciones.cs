using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundoRuta.BD.Migrations
{
    /// <inheritdoc />
    public partial class ArreglarFkCalificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Usuarios_UsuarioPrestadorId",
                table: "Calificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_UsuarioPrestadorId",
                table: "Calificaciones");

            migrationBuilder.DropColumn(
                name: "UsuarioPrestadorId",
                table: "Calificaciones");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_IdUsuarioPrestador",
                table: "Calificaciones",
                column: "IdUsuarioPrestador");

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Usuarios_IdUsuarioPrestador",
                table: "Calificaciones",
                column: "IdUsuarioPrestador",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Usuarios_IdUsuarioPrestador",
                table: "Calificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_IdUsuarioPrestador",
                table: "Calificaciones");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioPrestadorId",
                table: "Calificaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_UsuarioPrestadorId",
                table: "Calificaciones",
                column: "UsuarioPrestadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Usuarios_UsuarioPrestadorId",
                table: "Calificaciones",
                column: "UsuarioPrestadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
