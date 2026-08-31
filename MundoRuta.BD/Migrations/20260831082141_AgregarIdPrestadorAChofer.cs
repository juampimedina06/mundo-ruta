using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundoRuta.BD.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIdPrestadorAChofer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choferes_Usuarios_UsuarioId",
                table: "Choferes");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Choferes",
                newName: "IdPrestador");

            migrationBuilder.RenameIndex(
                name: "IX_Choferes_UsuarioId",
                table: "Choferes",
                newName: "IX_Choferes_IdPrestador");

            migrationBuilder.CreateIndex(
                name: "IX_Choferes_IdUsuario",
                table: "Choferes",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Choferes_Usuarios_IdPrestador",
                table: "Choferes",
                column: "IdPrestador",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Choferes_Usuarios_IdUsuario",
                table: "Choferes",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choferes_Usuarios_IdPrestador",
                table: "Choferes");

            migrationBuilder.DropForeignKey(
                name: "FK_Choferes_Usuarios_IdUsuario",
                table: "Choferes");

            migrationBuilder.DropIndex(
                name: "IX_Choferes_IdUsuario",
                table: "Choferes");

            migrationBuilder.RenameColumn(
                name: "IdPrestador",
                table: "Choferes",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Choferes_IdPrestador",
                table: "Choferes",
                newName: "IX_Choferes_UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Choferes_Usuarios_UsuarioId",
                table: "Choferes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
