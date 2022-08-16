using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PC.Services.DL.Migrations
{
    public partial class v6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DteailsId",
                table: "CategoryHeader",
                newName: "DteailId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApprovalDateTime",
                table: "approval",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DteailId",
                table: "CategoryHeader",
                newName: "DteailsId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApprovalDateTime",
                table: "approval",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
