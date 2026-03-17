using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompiladorCPLUS.Migrations
{
    /// <inheritdoc />
    public partial class AgregarResultadoAnalisis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResultadoAnalisis",
                table: "Compilaciones",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultadoAnalisis",
                table: "Compilaciones");
        }
    }
}
