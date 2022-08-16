using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PC.Services.DL.Migrations
{
    public partial class v7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryHeader_Details_DetailsId",
                table: "CategoryHeader");

            migrationBuilder.DropColumn(
                name: "DteailId",
                table: "CategoryHeader");

            migrationBuilder.AlterColumn<int>(
                name: "DetailsId",
                table: "CategoryHeader",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryHeader_Details_DetailsId",
                table: "CategoryHeader",
                column: "DetailsId",
                principalTable: "Details",
                principalColumn: "DetailsId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryHeader_Details_DetailsId",
                table: "CategoryHeader");

            migrationBuilder.AlterColumn<int>(
                name: "DetailsId",
                table: "CategoryHeader",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DteailId",
                table: "CategoryHeader",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryHeader_Details_DetailsId",
                table: "CategoryHeader",
                column: "DetailsId",
                principalTable: "Details",
                principalColumn: "DetailsId");
        }
    }
}
