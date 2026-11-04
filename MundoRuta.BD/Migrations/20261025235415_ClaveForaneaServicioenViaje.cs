using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundoRuta.BD.Migrations
{
    /// <inheritdoc />
    public partial class ClaveForaneaServicioenViaje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Viajes_Servicios_ServicioId",
                table: "Viajes");

            migrationBuilder.DropIndex(
                name: "IX_Viajes_ServicioId",
                table: "Viajes");

            migrationBuilder.DropColumn(
                name: "ServicioId",
                table: "Viajes");

            migrationBuilder.CreateIndex(
                name: "IX_Viajes_IdServicio",
                table: "Viajes",
                column: "IdServicio");

            migrationBuilder.AddForeignKey(
                name: "FK_Viajes_Servicios_IdServicio",
                table: "Viajes",
                column: "IdServicio",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Viajes_Servicios_IdServicio",
                table: "Viajes");

            migrationBuilder.DropIndex(
                name: "IX_Viajes_IdServicio",
                table: "Viajes");

            migrationBuilder.AddColumn<int>(
                name: "ServicioId",
                table: "Viajes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Viajes_ServicioId",
                table: "Viajes",
                column: "ServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Viajes_Servicios_ServicioId",
                table: "Viajes",
                column: "ServicioId",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
