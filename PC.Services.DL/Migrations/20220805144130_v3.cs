using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PC.Services.DL.Migrations
{
    public partial class v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_MainCategory_MainCategoryId",
                table: "Activity");

            migrationBuilder.AlterColumn<int>(
                name: "MainCategoryId",
                table: "Activity",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_MainCategory_MainCategoryId",
                table: "Activity",
                column: "MainCategoryId",
                principalTable: "MainCategory",
                principalColumn: "MainCategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_MainCategory_MainCategoryId",
                table: "Activity");

            migrationBuilder.AlterColumn<int>(
                name: "MainCategoryId",
                table: "Activity",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_MainCategory_MainCategoryId",
                table: "Activity",
                column: "MainCategoryId",
                principalTable: "MainCategory",
                principalColumn: "MainCategoryId");
        }
    }
}
