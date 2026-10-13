using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiCoMet.Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTipoFondoTanque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoFondo",
                table: "Tanques",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoFondo",
                table: "Tanques");
        }
    }
}
