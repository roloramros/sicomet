using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiCoMet.Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAlturaPared_FactorVolumen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AlturaTotalPared",
                table: "Tanques",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FactorVolumen",
                table: "Tanques",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlturaTotalPared",
                table: "Tanques");

            migrationBuilder.DropColumn(
                name: "FactorVolumen",
                table: "Tanques");
        }
    }
}
