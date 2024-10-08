using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartParkCore.Migrations
{
    /// <inheritdoc />
    public partial class CorrezioneRicarica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Completata",
                table: "Ricariche",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IdPosto",
                table: "Ricariche",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "IdUtente",
                table: "Ricariche",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completata",
                table: "Ricariche");

            migrationBuilder.DropColumn(
                name: "IdPosto",
                table: "Ricariche");

            migrationBuilder.DropColumn(
                name: "IdUtente",
                table: "Ricariche");
        }
    }
}
