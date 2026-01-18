using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TroskoviRada.Migrations
{
    /// <inheritdoc />
    public partial class AddNaknadeToZaposlenik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ZaposlenikIdZaposlenik",
                table: "naknada",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_naknada_ZaposlenikIdZaposlenik",
                table: "naknada",
                column: "ZaposlenikIdZaposlenik");

            migrationBuilder.AddForeignKey(
                name: "FK_naknada_zaposlenik_ZaposlenikIdZaposlenik",
                table: "naknada",
                column: "ZaposlenikIdZaposlenik",
                principalTable: "zaposlenik",
                principalColumn: "IdZaposlenik");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_naknada_zaposlenik_ZaposlenikIdZaposlenik",
                table: "naknada");

            migrationBuilder.DropIndex(
                name: "IX_naknada_ZaposlenikIdZaposlenik",
                table: "naknada");

            migrationBuilder.DropColumn(
                name: "ZaposlenikIdZaposlenik",
                table: "naknada");
        }
    }
}
