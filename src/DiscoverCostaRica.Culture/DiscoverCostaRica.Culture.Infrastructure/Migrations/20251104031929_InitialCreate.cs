using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscoverCostaRica.Culture.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Culture.Dish",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ingredients = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preparation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Culture.Dish", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Culture.Tradition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Culture.Tradition", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Culture.Dish_Id",
                table: "Culture.Dish",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Culture.Tradition_Id",
                table: "Culture.Tradition",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Culture.Dish");

            migrationBuilder.DropTable(
                name: "Culture.Tradition");
        }
    }
}
