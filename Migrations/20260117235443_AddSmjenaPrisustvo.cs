using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TroskoviRada.Migrations
{
    /// <inheritdoc />
    public partial class AddSmjenaPrisustvo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "napomena",
                table: "evidencija_rada",
                newName: "Napomena");

            migrationBuilder.RenameColumn(
                name: "vrijeme_odlaska",
                table: "evidencija_rada",
                newName: "VrijemeOdlaska");

            migrationBuilder.RenameColumn(
                name: "vrijeme_dolaska",
                table: "evidencija_rada",
                newName: "VrijemeDolaska");

            migrationBuilder.RenameColumn(
                name: "ukupno_odradeno",
                table: "evidencija_rada",
                newName: "UkupnoOdradeno");

            migrationBuilder.RenameColumn(
                name: "id_zaposlenik",
                table: "evidencija_rada",
                newName: "IdZaposlenik");

            migrationBuilder.RenameColumn(
                name: "datum_rada",
                table: "evidencija_rada",
                newName: "Datum");

            migrationBuilder.RenameColumn(
                name: "id_evidencija",
                table: "evidencija_rada",
                newName: "IdPrisustvo");

            migrationBuilder.RenameIndex(
                name: "IX_evidencija_rada_id_zaposlenik",
                table: "evidencija_rada",
                newName: "IX_evidencija_rada_IdZaposlenik");

            migrationBuilder.AddColumn<decimal>(
                name: "BrojSati",
                table: "evidencija_rada",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdSmjena",
                table: "evidencija_rada",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_evidencija_rada_IdSmjena",
                table: "evidencija_rada",
                column: "IdSmjena");

            migrationBuilder.AddForeignKey(
                name: "FK_evidencija_rada_smjena_IdSmjena",
                table: "evidencija_rada",
                column: "IdSmjena",
                principalTable: "smjena",
                principalColumn: "IdSmjena");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_evidencija_rada_smjena_IdSmjena",
                table: "evidencija_rada");

            migrationBuilder.DropIndex(
                name: "IX_evidencija_rada_IdSmjena",
                table: "evidencija_rada");

            migrationBuilder.DropColumn(
                name: "BrojSati",
                table: "evidencija_rada");

            migrationBuilder.DropColumn(
                name: "IdSmjena",
                table: "evidencija_rada");

            migrationBuilder.RenameColumn(
                name: "Napomena",
                table: "evidencija_rada",
                newName: "napomena");

            migrationBuilder.RenameColumn(
                name: "VrijemeOdlaska",
                table: "evidencija_rada",
                newName: "vrijeme_odlaska");

            migrationBuilder.RenameColumn(
                name: "VrijemeDolaska",
                table: "evidencija_rada",
                newName: "vrijeme_dolaska");

            migrationBuilder.RenameColumn(
                name: "UkupnoOdradeno",
                table: "evidencija_rada",
                newName: "ukupno_odradeno");

            migrationBuilder.RenameColumn(
                name: "IdZaposlenik",
                table: "evidencija_rada",
                newName: "id_zaposlenik");

            migrationBuilder.RenameColumn(
                name: "Datum",
                table: "evidencija_rada",
                newName: "datum_rada");

            migrationBuilder.RenameColumn(
                name: "IdPrisustvo",
                table: "evidencija_rada",
                newName: "id_evidencija");

            migrationBuilder.RenameIndex(
                name: "IX_evidencija_rada_IdZaposlenik",
                table: "evidencija_rada",
                newName: "IX_evidencija_rada_id_zaposlenik");
        }
    }
}
