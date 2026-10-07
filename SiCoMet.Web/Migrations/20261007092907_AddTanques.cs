using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SiCoMet.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTanques : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tanques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: true),
                    AlturaReferencia = table.Column<decimal>(type: "numeric", nullable: true),
                    AlturaOperacional = table.Column<decimal>(type: "numeric", nullable: true),
                    AlturaMaximoLlenado = table.Column<decimal>(type: "numeric", nullable: true),
                    AlturaFondaje = table.Column<decimal>(type: "numeric", nullable: true),
                    AlturaPlatinaMedicion = table.Column<decimal>(type: "numeric", nullable: true),
                    AlturaCoronaConoFondo = table.Column<decimal>(type: "numeric", nullable: true),
                    DiametroNominal = table.Column<decimal>(type: "numeric", nullable: true),
                    TipoTecho = table.Column<int>(type: "integer", nullable: true),
                    CantidadRolos = table.Column<int>(type: "integer", nullable: true),
                    MasaTechoFlotante = table.Column<decimal>(type: "numeric", nullable: true),
                    FechaUltimaCalibracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NumeroCertificadoAforo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EntidadAforo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    VigenciaCertificado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tanques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tanques_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tanques_Numero",
                table: "Tanques",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tanques_ProductoId",
                table: "Tanques",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tanques");

            migrationBuilder.DropTable(
                name: "Productos");
        }
    }
}
