using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PC.Services.DL.Migrations
{
    public partial class v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_TrsDetails_TrsDetailsId",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryHeader_Activity_ActivityId",
                table: "CategoryHeader");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryHeader_MainCategory_MainCategoryId",
                table: "CategoryHeader");

            migrationBuilder.DropForeignKey(
                name: "FK_Details_Activity_ActivityId",
                table: "Details");

            migrationBuilder.DropForeignKey(
                name: "FK_TrsDetails_CategoryHeader_CategoryHeaderId",
                table: "TrsDetails");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryHeaderId",
                table: "TrsDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ActivityId",
                table: "Details",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MainCategoryId",
                table: "CategoryHeader",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ActivityId",
                table: "CategoryHeader",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DteailsId",
                table: "CategoryHeader",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TrsDetailsId",
                table: "Attachment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_TrsDetails_TrsDetailsId",
                table: "Attachment",
                column: "TrsDetailsId",
                principalTable: "TrsDetails",
                principalColumn: "TrsDetailsId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryHeader_Activity_ActivityId",
                table: "CategoryHeader",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryHeader_MainCategory_MainCategoryId",
                table: "CategoryHeader",
                column: "MainCategoryId",
                principalTable: "MainCategory",
                principalColumn: "MainCategoryId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Details_Activity_ActivityId",
                table: "Details",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrsDetails_CategoryHeader_CategoryHeaderId",
                table: "TrsDetails",
                column: "CategoryHeaderId",
                principalTable: "CategoryHeader",
                principalColumn: "CategoryHeaderId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_TrsDetails_TrsDetailsId",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryHeader_Activity_ActivityId",
                table: "CategoryHeader");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryHeader_MainCategory_MainCategoryId",
                table: "CategoryHeader");

            migrationBuilder.DropForeignKey(
                name: "FK_Details_Activity_ActivityId",
                table: "Details");

            migrationBuilder.DropForeignKey(
                name: "FK_TrsDetails_CategoryHeader_CategoryHeaderId",
                table: "TrsDetails");

            migrationBuilder.DropColumn(
                name: "DteailsId",
                table: "CategoryHeader");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryHeaderId",
                table: "TrsDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ActivityId",
                table: "Details",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MainCategoryId",
                table: "CategoryHeader",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ActivityId",
                table: "CategoryHeader",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TrsDetailsId",
                table: "Attachment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_TrsDetails_TrsDetailsId",
                table: "Attachment",
                column: "TrsDetailsId",
                principalTable: "TrsDetails",
                principalColumn: "TrsDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryHeader_Activity_ActivityId",
                table: "CategoryHeader",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryHeader_MainCategory_MainCategoryId",
                table: "CategoryHeader",
                column: "MainCategoryId",
                principalTable: "MainCategory",
                principalColumn: "MainCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Details_Activity_ActivityId",
                table: "Details",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrsDetails_CategoryHeader_CategoryHeaderId",
                table: "TrsDetails",
                column: "CategoryHeaderId",
                principalTable: "CategoryHeader",
                principalColumn: "CategoryHeaderId");
        }
    }
}
