using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TroskoviRada.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreatePostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "radno_mjesto",
                columns: table => new
                {
                    IdRadnoMjesto = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Naziv = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Opis = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Satnica = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_radno_mjesto", x => x.IdRadnoMjesto);
                });

            migrationBuilder.CreateTable(
                name: "smjena",
                columns: table => new
                {
                    IdSmjena = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NazivSmjene = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VrijemePocetka = table.Column<TimeSpan>(type: "interval", nullable: false),
                    VrijemeZavrsetka = table.Column<TimeSpan>(type: "interval", nullable: false),
                    KoeficijentPlacanja = table.Column<decimal>(type: "numeric(4,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_smjena", x => x.IdSmjena);
                });

            migrationBuilder.CreateTable(
                name: "tip_naknade",
                columns: table => new
                {
                    IdTipNaknade = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Naziv = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Opis = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Iznos = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    JePostotak = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tip_naknade", x => x.IdTipNaknade);
                });

            migrationBuilder.CreateTable(
                name: "tip_odsustva",
                columns: table => new
                {
                    id_tip_odsustva = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    koeficijent_isplate = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tip_odsustva", x => x.id_tip_odsustva);
                });

            migrationBuilder.CreateTable(
                name: "zaposlenik",
                columns: table => new
                {
                    IdZaposlenik = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ime = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Prezime = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    BrojRacuna = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Telefon = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Oib = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zaposlenik", x => x.IdZaposlenik);
                });

            migrationBuilder.CreateTable(
                name: "evidencija_rada",
                columns: table => new
                {
                    id_evidencija = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_zaposlenik = table.Column<int>(type: "integer", nullable: false),
                    datum_rada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    vrijeme_dolaska = table.Column<TimeSpan>(type: "interval", nullable: false),
                    vrijeme_odlaska = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ukupno_odradeno = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    napomena = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evidencija_rada", x => x.id_evidencija);
                    table.ForeignKey(
                        name: "fk_evidencija_rada_zaposlenik",
                        column: x => x.id_zaposlenik,
                        principalTable: "zaposlenik",
                        principalColumn: "IdZaposlenik",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Naknade",
                columns: table => new
                {
                    IdNaknada = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdZaposlenik = table.Column<int>(type: "integer", nullable: false),
                    IdTipNaknade = table.Column<int>(type: "integer", nullable: false),
                    Datum = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Iznos = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Napomena = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Naknade", x => x.IdNaknada);
                    table.ForeignKey(
                        name: "FK_Naknade_tip_naknade_IdTipNaknade",
                        column: x => x.IdTipNaknade,
                        principalTable: "tip_naknade",
                        principalColumn: "IdTipNaknade",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Naknade_zaposlenik_IdZaposlenik",
                        column: x => x.IdZaposlenik,
                        principalTable: "zaposlenik",
                        principalColumn: "IdZaposlenik",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "obracun",
                columns: table => new
                {
                    IdObračun = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdZaposlenik = table.Column<int>(type: "integer", nullable: false),
                    Mjesec = table.Column<int>(type: "integer", nullable: false),
                    Godina = table.Column<int>(type: "integer", nullable: false),
                    Osnovica = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    UkupneNaknade = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    NetoPlaca = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    Porez = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    BrutoPlaca = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    DatumObračuna = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Isplaceno = table.Column<bool>(type: "boolean", nullable: false),
                    Napomene = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_obracun", x => x.IdObračun);
                    table.ForeignKey(
                        name: "fk_obracun_zaposlenik",
                        column: x => x.IdZaposlenik,
                        principalTable: "zaposlenik",
                        principalColumn: "IdZaposlenik",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "odsustvo",
                columns: table => new
                {
                    id_odsustvo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_zaposlenik = table.Column<int>(type: "integer", nullable: false),
                    id_tip_odsustva = table.Column<int>(type: "integer", nullable: false),
                    datum_od = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    datum_do = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    razlog = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    odobreno = table.Column<bool>(type: "boolean", nullable: false),
                    TipOdsustvaIdTipOdsustva = table.Column<int>(type: "integer", nullable: true),
                    ZaposlenikIdZaposlenik = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_odsustvo", x => x.id_odsustvo);
                    table.ForeignKey(
                        name: "FK_odsustvo_tip_odsustva_TipOdsustvaIdTipOdsustva",
                        column: x => x.TipOdsustvaIdTipOdsustva,
                        principalTable: "tip_odsustva",
                        principalColumn: "id_tip_odsustva");
                    table.ForeignKey(
                        name: "FK_odsustvo_zaposlenik_ZaposlenikIdZaposlenik",
                        column: x => x.ZaposlenikIdZaposlenik,
                        principalTable: "zaposlenik",
                        principalColumn: "IdZaposlenik");
                    table.ForeignKey(
                        name: "fk_odsustvo_tip_odsustva",
                        column: x => x.id_tip_odsustva,
                        principalTable: "tip_odsustva",
                        principalColumn: "id_tip_odsustva",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_odsustvo_zaposlenik",
                        column: x => x.id_zaposlenik,
                        principalTable: "zaposlenik",
                        principalColumn: "IdZaposlenik",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "zaposlenje",
                columns: table => new
                {
                    IdZaposlenje = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdZaposlenik = table.Column<int>(type: "integer", nullable: false),
                    IdRadnoMjesto = table.Column<int>(type: "integer", nullable: false),
                    DatumOd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DatumDo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zaposlenje", x => x.IdZaposlenje);
                    table.ForeignKey(
                        name: "fk_zaposlenje_radno_mjesto",
                        column: x => x.IdRadnoMjesto,
                        principalTable: "radno_mjesto",
                        principalColumn: "IdRadnoMjesto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_zaposlenje_zaposlenik",
                        column: x => x.IdZaposlenik,
                        principalTable: "zaposlenik",
                        principalColumn: "IdZaposlenik",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_evidencija_rada_id_zaposlenik",
                table: "evidencija_rada",
                column: "id_zaposlenik");

            migrationBuilder.CreateIndex(
                name: "IX_Naknade_IdTipNaknade",
                table: "Naknade",
                column: "IdTipNaknade");

            migrationBuilder.CreateIndex(
                name: "IX_Naknade_IdZaposlenik",
                table: "Naknade",
                column: "IdZaposlenik");

            migrationBuilder.CreateIndex(
                name: "IX_obracun_IdZaposlenik",
                table: "obracun",
                column: "IdZaposlenik");

            migrationBuilder.CreateIndex(
                name: "IX_odsustvo_id_tip_odsustva",
                table: "odsustvo",
                column: "id_tip_odsustva");

            migrationBuilder.CreateIndex(
                name: "IX_odsustvo_id_zaposlenik",
                table: "odsustvo",
                column: "id_zaposlenik");

            migrationBuilder.CreateIndex(
                name: "IX_odsustvo_TipOdsustvaIdTipOdsustva",
                table: "odsustvo",
                column: "TipOdsustvaIdTipOdsustva");

            migrationBuilder.CreateIndex(
                name: "IX_odsustvo_ZaposlenikIdZaposlenik",
                table: "odsustvo",
                column: "ZaposlenikIdZaposlenik");

            migrationBuilder.CreateIndex(
                name: "IX_zaposlenje_IdRadnoMjesto",
                table: "zaposlenje",
                column: "IdRadnoMjesto");

            migrationBuilder.CreateIndex(
                name: "IX_zaposlenje_IdZaposlenik",
                table: "zaposlenje",
                column: "IdZaposlenik");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "evidencija_rada");

            migrationBuilder.DropTable(
                name: "Naknade");

            migrationBuilder.DropTable(
                name: "obracun");

            migrationBuilder.DropTable(
                name: "odsustvo");

            migrationBuilder.DropTable(
                name: "smjena");

            migrationBuilder.DropTable(
                name: "zaposlenje");

            migrationBuilder.DropTable(
                name: "tip_naknade");

            migrationBuilder.DropTable(
                name: "tip_odsustva");

            migrationBuilder.DropTable(
                name: "radno_mjesto");

            migrationBuilder.DropTable(
                name: "zaposlenik");
        }
    }
}
