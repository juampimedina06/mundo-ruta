using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundoRuta.BD.Migrations
{
    /// <inheritdoc />
    public partial class AgregarVolumenYPeso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Peso",
                table: "Viajes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Volumen",
                table: "Viajes",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Peso",
                table: "Viajes");

            migrationBuilder.DropColumn(
                name: "Volumen",
                table: "Viajes");
        }
    }
}
