using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundoRuta.BD.Migrations
{
    /// <inheritdoc />
    public partial class IdVehiculoOpcional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Viajes_Vehiculos_IdVehiculo",
                table: "Viajes");

            migrationBuilder.AlterColumn<int>(
                name: "IdVehiculo",
                table: "Viajes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Viajes_Vehiculos_IdVehiculo",
                table: "Viajes",
                column: "IdVehiculo",
                principalTable: "Vehiculos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Viajes_Vehiculos_IdVehiculo",
                table: "Viajes");

            migrationBuilder.AlterColumn<int>(
                name: "IdVehiculo",
                table: "Viajes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Viajes_Vehiculos_IdVehiculo",
                table: "Viajes",
                column: "IdVehiculo",
                principalTable: "Vehiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
