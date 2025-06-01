using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinyHouseRezervasyon.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEvSahibiNavigationAndOzellikler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuncellemeTarihi",
                table: "Evler");

            migrationBuilder.DropColumn(
                name: "Metrekare",
                table: "Evler");

            migrationBuilder.AlterColumn<string>(
                name: "ResimUrl",
                table: "Evler",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ozellikler",
                table: "Evler",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvSahibiAdi",
                table: "Evler",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ilce",
                table: "Evler",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "EvFotograflari",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AnaFotograf",
                table: "EvFotograflari",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "YuklemeTarihi",
                table: "EvFotograflari",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EvSahibiAdi",
                table: "Evler");

            migrationBuilder.DropColumn(
                name: "Ilce",
                table: "Evler");

            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "EvFotograflari");

            migrationBuilder.DropColumn(
                name: "AnaFotograf",
                table: "EvFotograflari");

            migrationBuilder.DropColumn(
                name: "YuklemeTarihi",
                table: "EvFotograflari");

            migrationBuilder.AlterColumn<string>(
                name: "ResimUrl",
                table: "Evler",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Ozellikler",
                table: "Evler",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTime>(
                name: "GuncellemeTarihi",
                table: "Evler",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Metrekare",
                table: "Evler",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
