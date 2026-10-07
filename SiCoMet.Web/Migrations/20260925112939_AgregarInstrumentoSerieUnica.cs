using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiCoMet.Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarInstrumentoSerieUnica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Instrumentos_Serie",
                table: "Instrumentos",
                column: "Serie",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Instrumentos_Serie",
                table: "Instrumentos");
        }
    }
}
