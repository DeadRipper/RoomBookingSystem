using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RBA.DBase.Migrations
{
    /// <inheritdoc />
    public partial class reserRoomfffff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AmenityModel_AmenitiesId",
                table: "Rooms");

            migrationBuilder.RenameTable(
                name: "AmenityModel",
                newName: "Amenities");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Amenities_AmenitiesId",
                table: "Rooms",
                column: "AmenitiesId",
                principalTable: "Amenities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RoomId",
                table: "Reservations",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Rooms_RoomId",
                table: "Reservations",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Rooms_RoomId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_RoomId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Amenities_AmenitiesId",
                table: "Rooms");

            migrationBuilder.RenameTable(
                name: "Amenities",
                newName: "AmenityModel");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AmenityModel_AmenitiesId",
                table: "Rooms",
                column: "AmenitiesId",
                principalTable: "AmenityModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
