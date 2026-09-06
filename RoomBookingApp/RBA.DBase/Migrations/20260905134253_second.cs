using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RBA.DBase.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "Rooms",
                newName: "Floor");

            migrationBuilder.AddColumn<int>(
                name: "AmenitiesId",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AmenityModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmenityModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AmenitiesId",
                table: "Rooms",
                column: "AmenitiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AmenityModel_AmenitiesId",
                table: "Rooms",
                column: "AmenitiesId",
                principalTable: "AmenityModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AmenityModel_AmenitiesId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "AmenityModel");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_AmenitiesId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "AmenitiesId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "Floor",
                table: "Rooms",
                newName: "RoomId");
        }
    }
}
