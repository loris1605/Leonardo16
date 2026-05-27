using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class Fidelities2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsChecked",
                table: "FidelityEntries");

            migrationBuilder.RenameColumn(
                name: "EntryBeforeFree",
                table: "TipiFidelity",
                newName: "TipoFidelityOutputId");

            migrationBuilder.RenameColumn(
                name: "DataIngresso",
                table: "FidelityEntries",
                newName: "Value");

            migrationBuilder.AddColumn<int>(
                name: "TipoFidelityInputId",
                table: "TipiFidelity",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DataOra",
                table: "FidelityEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TipiFidelityInput",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipiFidelityInput", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipiFidelityOutput",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipiFidelityOutput", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TipiFidelityInput",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Ingressi" },
                    { 2, "Incassi" }
                });

            migrationBuilder.InsertData(
                table: "TipiFidelityOutput",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Ingressi" },
                    { 2, "Sconto" },
                    { 3, "Premio" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TipiFidelity_TipoFidelityInputId",
                table: "TipiFidelity",
                column: "TipoFidelityInputId");

            migrationBuilder.CreateIndex(
                name: "IX_TipiFidelity_TipoFidelityOutputId",
                table: "TipiFidelity",
                column: "TipoFidelityOutputId");

            migrationBuilder.AddForeignKey(
                name: "FK_TipiFidelity_TipiFidelityInput_TipoFidelityInputId",
                table: "TipiFidelity",
                column: "TipoFidelityInputId",
                principalTable: "TipiFidelityInput",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TipiFidelity_TipiFidelityOutput_TipoFidelityOutputId",
                table: "TipiFidelity",
                column: "TipoFidelityOutputId",
                principalTable: "TipiFidelityOutput",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TipiFidelity_TipiFidelityInput_TipoFidelityInputId",
                table: "TipiFidelity");

            migrationBuilder.DropForeignKey(
                name: "FK_TipiFidelity_TipiFidelityOutput_TipoFidelityOutputId",
                table: "TipiFidelity");

            migrationBuilder.DropTable(
                name: "TipiFidelityInput");

            migrationBuilder.DropTable(
                name: "TipiFidelityOutput");

            migrationBuilder.DropIndex(
                name: "IX_TipiFidelity_TipoFidelityInputId",
                table: "TipiFidelity");

            migrationBuilder.DropIndex(
                name: "IX_TipiFidelity_TipoFidelityOutputId",
                table: "TipiFidelity");

            migrationBuilder.DropColumn(
                name: "TipoFidelityInputId",
                table: "TipiFidelity");

            migrationBuilder.DropColumn(
                name: "DataOra",
                table: "FidelityEntries");

            migrationBuilder.RenameColumn(
                name: "TipoFidelityOutputId",
                table: "TipiFidelity",
                newName: "EntryBeforeFree");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "FidelityEntries",
                newName: "DataIngresso");

            migrationBuilder.AddColumn<bool>(
                name: "IsChecked",
                table: "FidelityEntries",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
