using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundoRuta.BD.Migrations
{
    /// <inheritdoc />
    public partial class AgregarVehiculoPropuestoOferta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdVehiculoPropuesto",
                table: "OfertasViajes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfertasViajes_IdVehiculoPropuesto",
                table: "OfertasViajes",
                column: "IdVehiculoPropuesto");

            migrationBuilder.AddForeignKey(
                name: "FK_OfertasViajes_Vehiculos_IdVehiculoPropuesto",
                table: "OfertasViajes",
                column: "IdVehiculoPropuesto",
                principalTable: "Vehiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfertasViajes_Vehiculos_IdVehiculoPropuesto",
                table: "OfertasViajes");

            migrationBuilder.DropIndex(
                name: "IX_OfertasViajes_IdVehiculoPropuesto",
                table: "OfertasViajes");

            migrationBuilder.DropColumn(
                name: "IdVehiculoPropuesto",
                table: "OfertasViajes");
        }
    }
}
