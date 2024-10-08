using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartParkCore.Migrations
{
    /// <inheritdoc />
    public partial class AggiuntaRuoloUtente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ruolo",
                table: "Utenti",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ruolo",
                table: "Utenti");
        }
    }
}
