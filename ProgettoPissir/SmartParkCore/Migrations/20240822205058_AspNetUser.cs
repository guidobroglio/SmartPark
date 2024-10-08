using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartParkCore.Migrations
{
    /// <inheritdoc />
    public partial class AspNetUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ruolo",
                table: "Utenti",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Utenti",
                newName: "SecurityStamp");

            migrationBuilder.AlterColumn<string>(
                name: "Targa",
                table: "Veicoli",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Utenti",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Utenti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Utenti",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Utenti",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "Utenti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Utenti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Utenti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Utenti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Utenti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Utenti",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Utenti",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "Utenti");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Utenti");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Utenti");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "Utenti");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Utenti");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Utenti");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Utenti");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Utenti");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Utenti");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Utenti");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Utenti");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Utenti",
                newName: "Ruolo");

            migrationBuilder.RenameColumn(
                name: "SecurityStamp",
                table: "Utenti",
                newName: "Password");

            migrationBuilder.AlterColumn<string>(
                name: "Targa",
                table: "Veicoli",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
