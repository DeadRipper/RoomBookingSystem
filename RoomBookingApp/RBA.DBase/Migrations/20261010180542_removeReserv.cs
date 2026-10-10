using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RBA.DBase.Migrations
{
    /// <inheritdoc />
    public partial class removeReserv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservationsId",
                table: "Rooms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReservationsId",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
