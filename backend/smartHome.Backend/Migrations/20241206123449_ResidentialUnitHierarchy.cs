using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smartHome.Backend.Migrations
{
    /// <inheritdoc />
    public partial class ResidentialUnitHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Apartments_ApartmentId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Houses_HouseId",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "Apartments");

            migrationBuilder.DropIndex(
                name: "IX_Devices_ApartmentId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_HouseId",
                table: "Devices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Houses",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "ApartmentId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "HouseId",
                table: "Devices");

            migrationBuilder.RenameTable(
                name: "Houses",
                newName: "ResidentialUnit");

            migrationBuilder.RenameColumn(
                name: "EnergyConumptionRate",
                table: "Devices",
                newName: "EnergyConsumptionRate");

            migrationBuilder.RenameColumn(
                name: "houseId",
                table: "ResidentialUnit",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "ResidentialUnitId",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApartmentComplexId",
                table: "ResidentialUnit",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResidentialUnitType",
                table: "ResidentialUnit",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResidentialUnit",
                table: "ResidentialUnit",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ResidentialUnitId",
                table: "Devices",
                column: "ResidentialUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentialUnit_ApartmentComplexId",
                table: "ResidentialUnit",
                column: "ApartmentComplexId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_ResidentialUnit_ResidentialUnitId",
                table: "Devices",
                column: "ResidentialUnitId",
                principalTable: "ResidentialUnit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResidentialUnit_ApartmentComplexes_ApartmentComplexId",
                table: "ResidentialUnit",
                column: "ApartmentComplexId",
                principalTable: "ApartmentComplexes",
                principalColumn: "ApartmentComplexId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_ResidentialUnit_ResidentialUnitId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_ResidentialUnit_ApartmentComplexes_ApartmentComplexId",
                table: "ResidentialUnit");

            migrationBuilder.DropIndex(
                name: "IX_Devices_ResidentialUnitId",
                table: "Devices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResidentialUnit",
                table: "ResidentialUnit");

            migrationBuilder.DropIndex(
                name: "IX_ResidentialUnit_ApartmentComplexId",
                table: "ResidentialUnit");

            migrationBuilder.DropColumn(
                name: "ResidentialUnitId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ApartmentComplexId",
                table: "ResidentialUnit");

            migrationBuilder.DropColumn(
                name: "ResidentialUnitType",
                table: "ResidentialUnit");

            migrationBuilder.RenameTable(
                name: "ResidentialUnit",
                newName: "Houses");

            migrationBuilder.RenameColumn(
                name: "EnergyConsumptionRate",
                table: "Devices",
                newName: "EnergyConumptionRate");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Houses",
                newName: "houseId");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_Houses",
                table: "Houses",
                column: "houseId");

            migrationBuilder.CreateTable(
                name: "Apartments",
                columns: table => new
                {
                    ApartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApartmentComplexId = table.Column<int>(type: "int", nullable: false),
                    UnitNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
    }
}
