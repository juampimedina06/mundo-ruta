using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundoRuta.BD.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarEntidadesFletesEstadosTarifas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Viajes_IdViajeId",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "IdViajeId",
                table: "Chats",
                newName: "IdViaje");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_IdViajeId",
                table: "Chats",
                newName: "IX_Chats_IdViaje");

            migrationBuilder.AddColumn<bool>(
                name: "CalificadoPorChofer",
                table: "Viajes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CalificadoPorPasajero",
                table: "Viajes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DniReceptor",
                table: "Viajes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EsCubierto",
                table: "Viajes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LlevaAcompanante",
                table: "Viajes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UsaBaulera",
                table: "Viajes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Vehiculos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FotoPerfilUrl",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TarifaBaseKm",
                table: "Usuarios",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Viajes_IdViaje",
                table: "Chats",
                column: "IdViaje",
                principalTable: "Viajes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Viajes_IdViaje",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "CalificadoPorChofer",
                table: "Viajes");

            migrationBuilder.DropColumn(
                name: "CalificadoPorPasajero",
                table: "Viajes");

            migrationBuilder.DropColumn(
                name: "DniReceptor",
                table: "Viajes");

            migrationBuilder.DropColumn(
                name: "EsCubierto",
                table: "Viajes");

            migrationBuilder.DropColumn(
                name: "LlevaAcompanante",
                table: "Viajes");

            migrationBuilder.DropColumn(
                name: "UsaBaulera",
                table: "Viajes");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "FotoPerfilUrl",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TarifaBaseKm",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "IdViaje",
                table: "Chats",
                newName: "IdViajeId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_IdViaje",
                table: "Chats",
                newName: "IX_Chats_IdViajeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Viajes_IdViajeId",
                table: "Chats",
                column: "IdViajeId",
                principalTable: "Viajes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
