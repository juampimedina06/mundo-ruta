using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundoRuta.BD.Migrations
{
    /// <inheritdoc />
    public partial class CrearEntidadOfertaViaje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OfertasViajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdViaje = table.Column<int>(type: "int", nullable: false),
                    IdPrestador = table.Column<int>(type: "int", nullable: false),
                    IdChoferPropuesto = table.Column<int>(type: "int", nullable: true),
                    MontoOfertado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EsContraoferta = table.Column<bool>(type: "bit", nullable: false),
                    EstadoOferta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaOferta = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfertasViajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfertasViajes_Choferes_IdChoferPropuesto",
                        column: x => x.IdChoferPropuesto,
                        principalTable: "Choferes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfertasViajes_Usuarios_IdPrestador",
                        column: x => x.IdPrestador,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfertasViajes_Viajes_IdViaje",
                        column: x => x.IdViaje,
                        principalTable: "Viajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfertasViajes_IdChoferPropuesto",
                table: "OfertasViajes",
                column: "IdChoferPropuesto");

            migrationBuilder.CreateIndex(
                name: "IX_OfertasViajes_IdPrestador",
                table: "OfertasViajes",
                column: "IdPrestador");

            migrationBuilder.CreateIndex(
                name: "IX_OfertasViajes_IdViaje",
                table: "OfertasViajes",
                column: "IdViaje");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfertasViajes");
        }
    }
}
