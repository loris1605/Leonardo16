using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Giornate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataInizio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFine = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aperta = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Giornate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SurName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Natoil = table.Column<int>(type: "int", nullable: false),
                    UniqueParam = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Version = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Strisciate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodiceSocio = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    DataOra = table.Column<int>(type: "int", nullable: false),
                    NumeroTessera = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Cognome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Natoil = table.Column<int>(type: "int", nullable: false),
                    Scadenza = table.Column<int>(type: "int", nullable: false),
                    CodMotivo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Strisciate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tariffe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    IsFreeDrink = table.Column<bool>(type: "bit", nullable: false),
                    Prezzo = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tariffe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipiPostazione",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipiPostazione", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipiRientro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    DurataOre = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipiRientro", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipiSettore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipiSettore", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Operatori",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Abilitato = table.Column<bool>(type: "bit", nullable: false),
                    Pass = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operatori", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operatori_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schede",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Posizione = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroTessera = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Cognome = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Natoil = table.Column<int>(type: "int", nullable: false),
                    CheckinTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckoutTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Grb1 = table.Column<int>(type: "int", nullable: false),
                    Grb2 = table.Column<int>(type: "int", nullable: false),
                    Grb3 = table.Column<int>(type: "int", nullable: false),
                    Grb4 = table.Column<int>(type: "int", nullable: false),
                    Consumazione = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Blocco = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schede", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schede_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Soci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroSocio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Abilitato = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Soci", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Soci_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Postazioni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    TipoPostazioneId = table.Column<int>(type: "int", nullable: false),
                    TipoRientroId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postazioni", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Postazioni_TipiPostazione_TipoPostazioneId",
                        column: x => x.TipoPostazioneId,
                        principalTable: "TipiPostazione",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Postazioni_TipiRientro_TipoRientroId",
                        column: x => x.TipoRientroId,
                        principalTable: "TipiRientro",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Settori",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    TipoSettoreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settori", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Settori_TipiSettore_TipoSettoreId",
                        column: x => x.TipoSettoreId,
                        principalTable: "TipiSettore",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tessere",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroTessera = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Scadenza = table.Column<int>(type: "int", nullable: false),
                    SocioId = table.Column<int>(type: "int", nullable: false),
                    Abilitato = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tessere", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tessere_Soci_SocioId",
                        column: x => x.SocioId,
                        principalTable: "Soci",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Permessi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperatoreId = table.Column<int>(type: "int", nullable: false),
                    PostazioneId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permessi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permessi_Operatori_OperatoreId",
                        column: x => x.OperatoreId,
                        principalTable: "Operatori",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Permessi_Postazioni_PostazioneId",
                        column: x => x.PostazioneId,
                        principalTable: "Postazioni",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Listini",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SettoreId = table.Column<int>(type: "int", nullable: false),
                    TariffaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listini", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Listini_Settori_SettoreId",
                        column: x => x.SettoreId,
                        principalTable: "Settori",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Listini_Tariffe_TariffaId",
                        column: x => x.TariffaId,
                        principalTable: "Tariffe",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reparti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SettoreId = table.Column<int>(type: "int", nullable: false),
                    PostazioneId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reparti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reparti_Postazioni_PostazioneId",
                        column: x => x.PostazioneId,
                        principalTable: "Postazioni",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reparti_Settori_SettoreId",
                        column: x => x.SettoreId,
                        principalTable: "Settori",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "People",
                columns: new[] { "Id", "FirstName", "Natoil", "SurName", "UniqueParam" },
                values: new object[,]
                {
                    { -999, "Di Servizio", 21000101, "Tessera", "TESSER21000101" },
                    { -3, "Non Importato", 21000101, "Socio", "SCOSOC21000101" },
                    { -2, "SOCIO", 21000101, "AMMINISTRATORE", "AMMSOC21000101" },
                    { -1, "SOCIO", 21000101, "VIRTUALE", "VIRSOC21000101" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "UpdatedAt", "Version" },
                values: new object[] { -1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "TipiPostazione",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Amministratore" },
                    { 2, "Cassa" },
                    { 3, "Bar" },
                    { 4, "Guardaroba" },
                    { 5, "Pulizie" }
                });

            migrationBuilder.InsertData(
                table: "TipiRientro",
                columns: new[] { "Id", "DurataOre", "Nome" },
                values: new object[,]
                {
                    { -2, 0, "Giornata" },
                    { -1, 0, "Nessuno" }
                });

            migrationBuilder.InsertData(
                table: "TipiSettore",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { -2, "Standard" },
                    { -1, "Ingressi" }
                });

            migrationBuilder.InsertData(
                table: "Operatori",
                columns: new[] { "Id", "Abilitato", "Nome", "Pass", "Password", "PersonId" },
                values: new object[] { -1, true, "ADMIN", 0, "ADMIN", -2 });

            migrationBuilder.InsertData(
                table: "Postazioni",
                columns: new[] { "Id", "Nome", "TipoPostazioneId", "TipoRientroId" },
                values: new object[] { -1, "Amministratore base", 1, -1 });

            migrationBuilder.InsertData(
                table: "Permessi",
                columns: new[] { "Id", "OperatoreId", "PostazioneId" },
                values: new object[] { -1, -1, -1 });

            migrationBuilder.CreateIndex(
                name: "IX_Listini_SettoreId",
                table: "Listini",
                column: "SettoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Listini_TariffaId",
                table: "Listini",
                column: "TariffaId");

            migrationBuilder.CreateIndex(
                name: "IX_Operatori_PersonId",
                table: "Operatori",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_People_UniqueParam",
                table: "People",
                column: "UniqueParam",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Person_FirstName",
                table: "People",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Person_SurName",
                table: "People",
                column: "SurName");

            migrationBuilder.CreateIndex(
                name: "IX_Permessi_OperatoreId",
                table: "Permessi",
                column: "OperatoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Permessi_PostazioneId",
                table: "Permessi",
                column: "PostazioneId");

            migrationBuilder.CreateIndex(
                name: "IX_Postazioni_TipoPostazioneId",
                table: "Postazioni",
                column: "TipoPostazioneId");

            migrationBuilder.CreateIndex(
                name: "IX_Postazioni_TipoRientroId",
                table: "Postazioni",
                column: "TipoRientroId");

            migrationBuilder.CreateIndex(
                name: "IX_Reparti_PostazioneId",
                table: "Reparti",
                column: "PostazioneId");

            migrationBuilder.CreateIndex(
                name: "IX_Reparti_SettoreId",
                table: "Reparti",
                column: "SettoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Schede_PersonId",
                table: "Schede",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Settori_TipoSettoreId",
                table: "Settori",
                column: "TipoSettoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Soci_NumeroSocio",
                table: "Soci",
                column: "NumeroSocio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Soci_PersonId",
                table: "Soci",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Tessere_NumeroTessera",
                table: "Tessere",
                column: "NumeroTessera",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tessere_SocioId",
                table: "Tessere",
                column: "SocioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Giornate");

            migrationBuilder.DropTable(
                name: "Listini");

            migrationBuilder.DropTable(
                name: "Permessi");

            migrationBuilder.DropTable(
                name: "Reparti");

            migrationBuilder.DropTable(
                name: "Schede");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Strisciate");

            migrationBuilder.DropTable(
                name: "Tessere");

            migrationBuilder.DropTable(
                name: "Tariffe");

            migrationBuilder.DropTable(
                name: "Operatori");

            migrationBuilder.DropTable(
                name: "Postazioni");

            migrationBuilder.DropTable(
                name: "Settori");

            migrationBuilder.DropTable(
                name: "Soci");

            migrationBuilder.DropTable(
                name: "TipiPostazione");

            migrationBuilder.DropTable(
                name: "TipiRientro");

            migrationBuilder.DropTable(
                name: "TipiSettore");

            migrationBuilder.DropTable(
                name: "People");
        }
    }
}
