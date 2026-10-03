using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RBA.DBase.Migrations
{
    /// <inheritdoc />
    public partial class res3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Rooms_RoomModelId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_RoomModelId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "RoomModelId",
                table: "Reservations");

            migrationBuilder.AddColumn<string>(
                name: "ReservationsId",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservationsId",
                table: "Rooms");

            migrationBuilder.AddColumn<int>(
                name: "RoomModelId",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RoomModelId",
                table: "Reservations",
                column: "RoomModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Rooms_RoomModelId",
                table: "Reservations",
                column: "RoomModelId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }
    }
}
