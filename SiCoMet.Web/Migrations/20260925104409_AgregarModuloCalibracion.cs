using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SiCoMet.Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModuloCalibracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposInstrumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    UnidadMedida = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposInstrumento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instrumentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AreaId = table.Column<int>(type: "integer", nullable: false),
                    TipoInstrumentoId = table.Column<int>(type: "integer", nullable: false),
                    Serie = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Posicion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RangoMedicionMin = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    RangoMedicionMax = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    PeriodicidadMeses = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instrumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instrumentos_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Instrumentos_TiposInstrumento_TipoInstrumentoId",
                        column: x => x.TipoInstrumentoId,
                        principalTable: "TiposInstrumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Calibraciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstrumentoId = table.Column<int>(type: "integer", nullable: false),
                    FechaCalibracion = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaProximaCalibracion = table.Column<DateOnly>(type: "date", nullable: false),
                    CertificadoNumero = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CertificadoArchivoUrl = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    EstadoTecnico = table.Column<int>(type: "integer", nullable: false),
                    MotivoNoApto = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CalibradorId = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calibraciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Calibraciones_AspNetUsers_CalibradorId",
                        column: x => x.CalibradorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Calibraciones_Instrumentos_InstrumentoId",
                        column: x => x.InstrumentoId,
                        principalTable: "Instrumentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calibraciones_CalibradorId",
                table: "Calibraciones",
                column: "CalibradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Calibraciones_InstrumentoId",
                table: "Calibraciones",
                column: "InstrumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Instrumentos_AreaId",
                table: "Instrumentos",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Instrumentos_TipoInstrumentoId",
                table: "Instrumentos",
                column: "TipoInstrumentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calibraciones");

            migrationBuilder.DropTable(
                name: "Instrumentos");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "TiposInstrumento");
        }
    }
}
