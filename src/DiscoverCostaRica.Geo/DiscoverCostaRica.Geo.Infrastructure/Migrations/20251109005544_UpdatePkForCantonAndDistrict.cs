using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscoverCostaRica.Geo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePkForCantonAndDistrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Geo.District_Geo.Canton_CantonId",
                table: "Geo.District");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Geo.District",
                table: "Geo.District");

            migrationBuilder.DropIndex(
                name: "IX_Geo.District_CantonId",
                table: "Geo.District");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Geo.Canton",
                table: "Geo.Canton");

            migrationBuilder.AddColumn<int>(
                name: "CantonProvinceId",
                table: "Geo.District",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Geo.District",
                table: "Geo.District",
                columns: new[] { "Id", "CantonId", "CantonProvinceId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Geo.Canton",
                table: "Geo.Canton",
                columns: new[] { "Id", "ProvinceId" });

            migrationBuilder.CreateIndex(
                name: "IX_Geo.District_CantonId_CantonProvinceId",
                table: "Geo.District",
                columns: new[] { "CantonId", "CantonProvinceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Geo.District_Geo.Canton_CantonId_CantonProvinceId",
                table: "Geo.District",
                columns: new[] { "CantonId", "CantonProvinceId" },
                principalTable: "Geo.Canton",
                principalColumns: new[] { "Id", "ProvinceId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Geo.District_Geo.Canton_CantonId_CantonProvinceId",
                table: "Geo.District");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Geo.District",
                table: "Geo.District");

            migrationBuilder.DropIndex(
                name: "IX_Geo.District_CantonId_CantonProvinceId",
                table: "Geo.District");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Geo.Canton",
                table: "Geo.Canton");

            migrationBuilder.DropColumn(
                name: "CantonProvinceId",
                table: "Geo.District");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Geo.District",
                table: "Geo.District",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Geo.Canton",
                table: "Geo.Canton",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Geo.District_CantonId",
                table: "Geo.District",
                column: "CantonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Geo.District_Geo.Canton_CantonId",
                table: "Geo.District",
                column: "CantonId",
                principalTable: "Geo.Canton",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
