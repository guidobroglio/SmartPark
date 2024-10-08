using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartParkCore.Migrations
{
    /// <inheritdoc />
    public partial class CorrezioneSosta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PrezzoSosta",
                table: "Soste",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInizio",
                table: "Soste",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataFine",
                table: "Soste",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VeicoloId",
                table: "Soste",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrezzoRicarica",
                table: "Ricariche",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "ImportoSosta",
                table: "Pagamenti",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "ImportoRicarica",
                table: "Pagamenti",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.CreateIndex(
                name: "IX_Soste_VeicoloId",
                table: "Soste",
                column: "VeicoloId");

            migrationBuilder.AddForeignKey(
                name: "FK_Soste_Veicoli_VeicoloId",
                table: "Soste",
                column: "VeicoloId",
                principalTable: "Veicoli",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Soste_Veicoli_VeicoloId",
                table: "Soste");

            migrationBuilder.DropIndex(
                name: "IX_Soste_VeicoloId",
                table: "Soste");

            migrationBuilder.DropColumn(
                name: "VeicoloId",
                table: "Soste");

            migrationBuilder.AlterColumn<float>(
                name: "PrezzoSosta",
                table: "Soste",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInizio",
                table: "Soste",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataFine",
                table: "Soste",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<float>(
                name: "PrezzoRicarica",
                table: "Ricariche",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<float>(
                name: "ImportoSosta",
                table: "Pagamenti",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<float>(
                name: "ImportoRicarica",
                table: "Pagamenti",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");
        }
    }
}
