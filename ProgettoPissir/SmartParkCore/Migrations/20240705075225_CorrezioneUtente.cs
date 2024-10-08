using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartParkCore.Migrations
{
    /// <inheritdoc />
    public partial class CorrezioneUtente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veicoli_Utenti_UtenteId",
                table: "Veicoli");

            migrationBuilder.DropIndex(
                name: "IX_Veicoli_UtenteId",
                table: "Veicoli");

            migrationBuilder.DropColumn(
                name: "UtenteId",
                table: "Veicoli");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Utenti",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Utenti");

            migrationBuilder.AddColumn<Guid>(
                name: "UtenteId",
                table: "Veicoli",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Veicoli_UtenteId",
                table: "Veicoli",
                column: "UtenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Veicoli_Utenti_UtenteId",
                table: "Veicoli",
                column: "UtenteId",
                principalTable: "Utenti",
                principalColumn: "Id");
        }
    }
}
