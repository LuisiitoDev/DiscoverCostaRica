using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscoverCostaRica.Geo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Geo.Province",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Geo.Province", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Geo.Canton",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Geo.Canton", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Geo.Canton_Geo.Province_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Geo.Province",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Geo.District",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CantonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Geo.District", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Geo.District_Geo.Canton_CantonId",
                        column: x => x.CantonId,
                        principalTable: "Geo.Canton",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Geo.Canton_ProvinceId",
                table: "Geo.Canton",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Geo.District_CantonId",
                table: "Geo.District",
                column: "CantonId");

            migrationBuilder.CreateIndex(
                name: "IX_Geo.Province_Id",
                table: "Geo.Province",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Geo.District");

            migrationBuilder.DropTable(
                name: "Geo.Canton");

            migrationBuilder.DropTable(
                name: "Geo.Province");
        }
    }
}
