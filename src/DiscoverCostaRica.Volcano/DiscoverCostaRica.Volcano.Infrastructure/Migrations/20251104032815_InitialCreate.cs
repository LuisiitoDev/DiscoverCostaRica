using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscoverCostaRica.Volcano.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Volcano.Volcano",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    CantonId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Volcano.Volcano", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Volcano.Volcano_Id",
                table: "Volcano.Volcano",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Volcano.Volcano");
        }
    }
}
