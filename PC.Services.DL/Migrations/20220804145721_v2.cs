using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PC.Services.DL.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "TrsDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "TrsDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "MainCategory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "MainCategory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Levels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "Levels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "jobTitle",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "jobTitle",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Details",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "Details",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "CategoryHeader",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "CategoryHeader",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "AuthorityMatrix",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "AuthorityMatrix",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Attachment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "Attachment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "ApprovalStatus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "ApprovalStatus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "approval",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "approval",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "Activity",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Activity",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "TrsDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedDateTime",
                table: "TrsDetails");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "MainCategory");

            migrationBuilder.DropColumn(
                name: "UpdatedDateTime",
                table: "MainCategory");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Levels");

            migrationBuilder.DropColumn(
                name: "UpdatedDateTime",
                table: "Levels");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "jobTitle");

            migrationBuilder.DropColumn(
                name: "UpdatedDateTime",
                table: "jobTitle");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Details");

            migrationBuilder.DropColumn(
                name: "UpdatedDateTime",
                table: "Details");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "CategoryHeader");

            migrationBuilder.DropColumn(
                name: "UpdatedDateTime",
                table: "CategoryHeader");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "AuthorityMatrix");

            migrationBuilder.DropColumn(
                name: "UpdatedDateTime",
                table: "AuthorityMatrix");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "UpdatedDateTime",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "ApprovalStatus");

            migrationBuilder.DropColumn(
                name: "UpdatedDateTime",
                table: "ApprovalStatus");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "approval");

            migrationBuilder.DropColumn(
                name: "UpdatedDateTime",
                table: "approval");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "Activity",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Activity",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
