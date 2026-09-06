using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RBA.DBase.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Reservations_ReservationModelId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ReservationModelId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReservationModelId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UsersId",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UsersId",
                table: "Reservations",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_UsersId",
                table: "Reservations",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_UsersId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_UsersId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Reservations");

            migrationBuilder.AddColumn<int>(
                name: "ReservationModelId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ReservationModelId",
                table: "Users",
                column: "ReservationModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Reservations_ReservationModelId",
                table: "Users",
                column: "ReservationModelId",
                principalTable: "Reservations",
                principalColumn: "Id");
        }
    }
}
