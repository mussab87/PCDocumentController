using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PC.Services.DL.Migrations
{
    public partial class v5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_approval_ApprovalStatus_ApprovalStatusId1",
                table: "approval");

            migrationBuilder.DropForeignKey(
                name: "FK_approval_TrsDetails_TrsDetailsId",
                table: "approval");

            migrationBuilder.DropForeignKey(
                name: "FK_Levels_CategoryHeader_CategoryHeaderId",
                table: "Levels");

            migrationBuilder.DropIndex(
                name: "IX_approval_ApprovalStatusId1",
                table: "approval");

            migrationBuilder.DropColumn(
                name: "ApprovalStatusId1",
                table: "approval");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryHeaderId",
                table: "Levels",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TrsDetailsId",
                table: "approval",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ApprovalStatusId",
                table: "approval",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_approval_ApprovalStatusId",
                table: "approval",
                column: "ApprovalStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_approval_ApprovalStatus_ApprovalStatusId",
                table: "approval",
                column: "ApprovalStatusId",
                principalTable: "ApprovalStatus",
                principalColumn: "ApprovalStatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_approval_TrsDetails_TrsDetailsId",
                table: "approval",
                column: "TrsDetailsId",
                principalTable: "TrsDetails",
                principalColumn: "TrsDetailsId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Levels_CategoryHeader_CategoryHeaderId",
                table: "Levels",
                column: "CategoryHeaderId",
                principalTable: "CategoryHeader",
                principalColumn: "CategoryHeaderId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_approval_ApprovalStatus_ApprovalStatusId",
                table: "approval");

            migrationBuilder.DropForeignKey(
                name: "FK_approval_TrsDetails_TrsDetailsId",
                table: "approval");

            migrationBuilder.DropForeignKey(
                name: "FK_Levels_CategoryHeader_CategoryHeaderId",
                table: "Levels");

            migrationBuilder.DropIndex(
                name: "IX_approval_ApprovalStatusId",
                table: "approval");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryHeaderId",
                table: "Levels",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TrsDetailsId",
                table: "approval",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ApprovalStatusId",
                table: "approval",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatusId1",
                table: "approval",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_approval_ApprovalStatusId1",
                table: "approval",
                column: "ApprovalStatusId1");

            migrationBuilder.AddForeignKey(
                name: "FK_approval_ApprovalStatus_ApprovalStatusId1",
                table: "approval",
                column: "ApprovalStatusId1",
                principalTable: "ApprovalStatus",
                principalColumn: "ApprovalStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_approval_TrsDetails_TrsDetailsId",
                table: "approval",
                column: "TrsDetailsId",
                principalTable: "TrsDetails",
                principalColumn: "TrsDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Levels_CategoryHeader_CategoryHeaderId",
                table: "Levels",
                column: "CategoryHeaderId",
                principalTable: "CategoryHeader",
                principalColumn: "CategoryHeaderId");
        }
    }
}
