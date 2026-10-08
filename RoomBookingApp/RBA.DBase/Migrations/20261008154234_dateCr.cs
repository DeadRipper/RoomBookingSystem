using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RBA.DBase.Migrations
{
    /// <inheritdoc />
    public partial class dateCr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExparationDate",
                table: "Auths",
                newName: "ExpirationDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Auths",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Auths");

            migrationBuilder.RenameColumn(
                name: "ExpirationDate",
                table: "Auths",
                newName: "ExparationDate");
        }
    }
}
