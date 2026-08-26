using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundoRuta.BD.Migrations
{
    /// <inheritdoc />
    public partial class CambiostablaUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cuit",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RazonSocial",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cuit",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RazonSocial",
                table: "Usuarios");
        }
    }
}
