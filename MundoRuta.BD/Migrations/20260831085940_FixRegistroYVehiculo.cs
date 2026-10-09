using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundoRuta.BD.Migrations
{
    /// <inheritdoc />
    public partial class FixRegistroYVehiculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculos_Choferes_ChoferId",
                table: "Vehiculos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculos_Usuarios_UsuarioId",
                table: "Vehiculos");

            migrationBuilder.DropIndex(
                name: "IX_Vehiculos_ChoferId",
                table: "Vehiculos");

            migrationBuilder.DropIndex(
                name: "IX_Vehiculos_UsuarioId",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "ChoferId",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Vehiculos");

            migrationBuilder.AlterColumn<int>(
                name: "IdChofer",
                table: "Vehiculos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_IdChofer",
                table: "Vehiculos",
                column: "IdChofer");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_IdUsuario",
                table: "Vehiculos",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculos_Choferes_IdChofer",
                table: "Vehiculos",
                column: "IdChofer",
                principalTable: "Choferes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculos_Usuarios_IdUsuario",
                table: "Vehiculos",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculos_Choferes_IdChofer",
                table: "Vehiculos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculos_Usuarios_IdUsuario",
                table: "Vehiculos");

            migrationBuilder.DropIndex(
                name: "IX_Vehiculos_IdChofer",
                table: "Vehiculos");

            migrationBuilder.DropIndex(
                name: "IX_Vehiculos_IdUsuario",
                table: "Vehiculos");

            migrationBuilder.AlterColumn<int>(
                name: "IdChofer",
                table: "Vehiculos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChoferId",
                table: "Vehiculos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Vehiculos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_ChoferId",
                table: "Vehiculos",
                column: "ChoferId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_UsuarioId",
                table: "Vehiculos",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculos_Choferes_ChoferId",
                table: "Vehiculos",
                column: "ChoferId",
                principalTable: "Choferes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculos_Usuarios_UsuarioId",
                table: "Vehiculos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
