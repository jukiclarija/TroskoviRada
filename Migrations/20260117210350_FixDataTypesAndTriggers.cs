using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TroskoviRada.Migrations
{
    /// <inheritdoc />
    public partial class FixDataTypesAndTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Naknade_tip_naknade_IdTipNaknade",
                table: "Naknade");

            migrationBuilder.DropForeignKey(
                name: "FK_Naknade_zaposlenik_IdZaposlenik",
                table: "Naknade");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Naknade",
                table: "Naknade");

            migrationBuilder.RenameTable(
                name: "Naknade",
                newName: "naknada");

            migrationBuilder.RenameIndex(
                name: "IX_Naknade_IdZaposlenik",
                table: "naknada",
                newName: "IX_naknada_IdZaposlenik");

            migrationBuilder.RenameIndex(
                name: "IX_Naknade_IdTipNaknade",
                table: "naknada",
                newName: "IX_naknada_IdTipNaknade");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DatumOd",
                table: "zaposlenje",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DatumDo",
                table: "zaposlenje",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "koeficijent_isplate",
                table: "tip_odsustva",
                type: "numeric(3,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<DateTime>(
                name: "datum_od",
                table: "odsustvo",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "datum_do",
                table: "odsustvo",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DatumObračuna",
                table: "obracun",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "vrijeme_odlaska",
                table: "evidencija_rada",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "vrijeme_dolaska",
                table: "evidencija_rada",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AlterColumn<decimal>(
                name: "ukupno_odradeno",
                table: "evidencija_rada",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "datum_rada",
                table: "evidencija_rada",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Datum",
                table: "naknada",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_naknada",
                table: "naknada",
                column: "IdNaknada");

            migrationBuilder.AddForeignKey(
                name: "fk_naknada_tip_naknade",
                table: "naknada",
                column: "IdTipNaknade",
                principalTable: "tip_naknade",
                principalColumn: "IdTipNaknade",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_naknada_zaposlenik",
                table: "naknada",
                column: "IdZaposlenik",
                principalTable: "zaposlenik",
                principalColumn: "IdZaposlenik",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_naknada_tip_naknade",
                table: "naknada");

            migrationBuilder.DropForeignKey(
                name: "fk_naknada_zaposlenik",
                table: "naknada");

            migrationBuilder.DropPrimaryKey(
                name: "PK_naknada",
                table: "naknada");

            migrationBuilder.RenameTable(
                name: "naknada",
                newName: "Naknade");

            migrationBuilder.RenameIndex(
                name: "IX_naknada_IdZaposlenik",
                table: "Naknade",
                newName: "IX_Naknade_IdZaposlenik");

            migrationBuilder.RenameIndex(
                name: "IX_naknada_IdTipNaknade",
                table: "Naknade",
                newName: "IX_Naknade_IdTipNaknade");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DatumOd",
                table: "zaposlenje",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DatumDo",
                table: "zaposlenje",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "koeficijent_isplate",
                table: "tip_odsustva",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(3,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "datum_od",
                table: "odsustvo",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "datum_do",
                table: "odsustvo",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DatumObračuna",
                table: "obracun",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "vrijeme_odlaska",
                table: "evidencija_rada",
                type: "interval",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "vrijeme_dolaska",
                table: "evidencija_rada",
                type: "interval",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<decimal>(
                name: "ukupno_odradeno",
                table: "evidencija_rada",
                type: "numeric(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "datum_rada",
                table: "evidencija_rada",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Datum",
                table: "Naknade",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Naknade",
                table: "Naknade",
                column: "IdNaknada");

            migrationBuilder.AddForeignKey(
                name: "FK_Naknade_tip_naknade_IdTipNaknade",
                table: "Naknade",
                column: "IdTipNaknade",
                principalTable: "tip_naknade",
                principalColumn: "IdTipNaknade",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Naknade_zaposlenik_IdZaposlenik",
                table: "Naknade",
                column: "IdZaposlenik",
                principalTable: "zaposlenik",
                principalColumn: "IdZaposlenik",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
