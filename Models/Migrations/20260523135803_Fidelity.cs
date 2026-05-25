using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class Fidelity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TipiFidelity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    EntryBeforeFree = table.Column<int>(type: "int", nullable: false),
                    DurataGG = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipiFidelity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fidelities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoFidelityId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    DataAttivazione = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fidelities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fidelities_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Fidelities_TipiFidelity_TipoFidelityId",
                        column: x => x.TipoFidelityId,
                        principalTable: "TipiFidelity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FidelityEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FidelityId = table.Column<int>(type: "int", nullable: false),
                    IsChecked = table.Column<bool>(type: "bit", nullable: false),
                    DataIngresso = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FidelityEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FidelityEntries_Fidelities_FidelityId",
                        column: x => x.FidelityId,
                        principalTable: "Fidelities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fidelities_PersonId",
                table: "Fidelities",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fidelities_TipoFidelityId",
                table: "Fidelities",
                column: "TipoFidelityId");

            migrationBuilder.CreateIndex(
                name: "IX_FidelityEntries_FidelityId",
                table: "FidelityEntries",
                column: "FidelityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FidelityEntries");

            migrationBuilder.DropTable(
                name: "Fidelities");

            migrationBuilder.DropTable(
                name: "TipiFidelity");
        }
    }
}
