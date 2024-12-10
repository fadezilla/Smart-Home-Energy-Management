using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smartHome.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddHouseApartmentComplex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApartmentId",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HouseId",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApartmentComplexes",
                columns: table => new
                {
                    ApartmentComplexId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmentComplexes", x => x.ApartmentComplexId);
                });

            migrationBuilder.CreateTable(
                name: "Houses",
                columns: table => new
                {
                    houseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Houses", x => x.houseId);
                });

            migrationBuilder.CreateTable(
                name: "Apartments",
                columns: table => new
                {
                    ApartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApartmentComplexId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartments", x => x.ApartmentId);
                    table.ForeignKey(
                        name: "FK_Apartments_ApartmentComplexes_ApartmentComplexId",
                        column: x => x.ApartmentComplexId,
                        principalTable: "ApartmentComplexes",
                        principalColumn: "ApartmentComplexId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ApartmentId",
                table: "Devices",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_HouseId",
                table: "Devices",
                column: "HouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_ApartmentComplexId",
                table: "Apartments",
                column: "ApartmentComplexId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Apartments_ApartmentId",
                table: "Devices",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "ApartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Houses_HouseId",
                table: "Devices",
                column: "HouseId",
                principalTable: "Houses",
                principalColumn: "houseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Apartments_ApartmentId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Houses_HouseId",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "Apartments");

            migrationBuilder.DropTable(
                name: "Houses");

            migrationBuilder.DropTable(
                name: "ApartmentComplexes");

            migrationBuilder.DropIndex(
                name: "IX_Devices_ApartmentId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_HouseId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ApartmentId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "HouseId",
                table: "Devices");
        }
    }
}
