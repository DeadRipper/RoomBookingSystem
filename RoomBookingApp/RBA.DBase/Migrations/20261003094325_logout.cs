using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RBA.DBase.Migrations
{
    /// <inheritdoc />
    public partial class logout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentlyIn",
                table: "AdminInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LoginDate",
                table: "AdminInfo",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LogoutDate",
                table: "AdminInfo",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentlyIn",
                table: "AdminInfo");

            migrationBuilder.DropColumn(
                name: "LoginDate",
                table: "AdminInfo");

            migrationBuilder.DropColumn(
                name: "LogoutDate",
                table: "AdminInfo");
        }
    }
}
