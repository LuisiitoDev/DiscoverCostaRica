using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscoverCostaRica.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class CountryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Province",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryCode = table.Column<int>(type: "int", nullable: false),
                    Population = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Province_CountryId",
                table: "Province",
                column: "CountryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Province_Id",
                table: "Province",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Province_Countries_CountryId",
                table: "Province",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Province_Countries_CountryId",
                table: "Province");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Province_CountryId",
                table: "Province");

            migrationBuilder.DropIndex(
                name: "IX_Province_Id",
                table: "Province");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Province");
        }
    }
}
