using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundoRuta.BD.Migrations
{
    /// <inheritdoc />
    public partial class ArreglarFkMensajes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Chats_ChatId",
                table: "Mensajes");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Usuarios_DestinatarioId",
                table: "Mensajes");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Usuarios_EmisorId",
                table: "Mensajes");

            migrationBuilder.DropIndex(
                name: "IX_Mensajes_ChatId",
                table: "Mensajes");

            migrationBuilder.DropIndex(
                name: "IX_Mensajes_DestinatarioId",
                table: "Mensajes");

            migrationBuilder.DropIndex(
                name: "IX_Mensajes_EmisorId",
                table: "Mensajes");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Mensajes");

            migrationBuilder.DropColumn(
                name: "DestinatarioId",
                table: "Mensajes");

            migrationBuilder.DropColumn(
                name: "EmisorId",
                table: "Mensajes");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_IdChat",
                table: "Mensajes",
                column: "IdChat");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_IdDestinatario",
                table: "Mensajes",
                column: "IdDestinatario");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_IdEmisor",
                table: "Mensajes",
                column: "IdEmisor");

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Chats_IdChat",
                table: "Mensajes",
                column: "IdChat",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Usuarios_IdDestinatario",
                table: "Mensajes",
                column: "IdDestinatario",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Usuarios_IdEmisor",
                table: "Mensajes",
                column: "IdEmisor",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Chats_IdChat",
                table: "Mensajes");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Usuarios_IdDestinatario",
                table: "Mensajes");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Usuarios_IdEmisor",
                table: "Mensajes");

            migrationBuilder.DropIndex(
                name: "IX_Mensajes_IdChat",
                table: "Mensajes");

            migrationBuilder.DropIndex(
                name: "IX_Mensajes_IdDestinatario",
                table: "Mensajes");

            migrationBuilder.DropIndex(
                name: "IX_Mensajes_IdEmisor",
                table: "Mensajes");
        }
    }
}