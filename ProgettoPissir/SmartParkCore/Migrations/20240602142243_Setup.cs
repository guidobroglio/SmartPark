using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartParkCore.Migrations
{
    /// <inheritdoc />
    public partial class Setup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prenotazioni",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdUtente = table.Column<Guid>(type: "TEXT", nullable: false),
                    DataInizio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataFine = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Stato = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prenotazioni", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Soste",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdUtente = table.Column<Guid>(type: "TEXT", nullable: false),
                    PrezzoSosta = table.Column<float>(type: "REAL", nullable: false),
                    Durata = table.Column<int>(type: "INTEGER", nullable: false),
                    DataInizio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataFine = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Soste", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utenti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Cognome = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    NumeroCarta = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utenti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veicoli",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Targa = table.Column<string>(type: "TEXT", nullable: false),
                    CapacitaBatteria = table.Column<int>(type: "INTEGER", nullable: false),
                    PercentualeBatteria = table.Column<int>(type: "INTEGER", nullable: false),
                    UtenteId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veicoli", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veicoli_Utenti_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "Utenti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Pagamenti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdUtente = table.Column<Guid>(type: "TEXT", nullable: false),
                    ImportoSosta = table.Column<float>(type: "REAL", nullable: false),
                    ImportoRicarica = table.Column<float>(type: "REAL", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    VeicoloId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamenti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagamenti_Veicoli_VeicoloId",
                        column: x => x.VeicoloId,
                        principalTable: "Veicoli",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ricariche",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PrezzoRicarica = table.Column<float>(type: "REAL", nullable: false),
                    PercentualeRicarica = table.Column<int>(type: "INTEGER", nullable: false),
                    VeicoloId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataInizio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataFine = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ricariche", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ricariche_Veicoli_VeicoloId",
                        column: x => x.VeicoloId,
                        principalTable: "Veicoli",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pagamenti_VeicoloId",
                table: "Pagamenti",
                column: "VeicoloId");

            migrationBuilder.CreateIndex(
                name: "IX_Ricariche_VeicoloId",
                table: "Ricariche",
                column: "VeicoloId");

            migrationBuilder.CreateIndex(
                name: "IX_Veicoli_UtenteId",
                table: "Veicoli",
                column: "UtenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pagamenti");

            migrationBuilder.DropTable(
                name: "Prenotazioni");

            migrationBuilder.DropTable(
                name: "Ricariche");

            migrationBuilder.DropTable(
                name: "Soste");

            migrationBuilder.DropTable(
                name: "Veicoli");

            migrationBuilder.DropTable(
                name: "Utenti");
        }
    }
}
