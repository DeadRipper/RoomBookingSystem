using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RBA.DBase.Migrations
{
    /// <inheritdoc />
    public partial class res : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Rooms_RoomId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_UsersId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_RoomId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_UsersId",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "Reservations",
                newName: "UserId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reservations",
                newName: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RoomId",
                table: "Reservations",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UsersId",
                table: "Reservations",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Rooms_RoomId",
                table: "Reservations",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_UsersId",
                table: "Reservations",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
