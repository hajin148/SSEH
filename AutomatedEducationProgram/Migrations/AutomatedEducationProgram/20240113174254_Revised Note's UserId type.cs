using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutomatedEducationProgram.Migrations.AutomatedEducationProgram
{
    public partial class RevisedNotesUserIdtype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_AEPUser_UserId",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_UserId",
                table: "Note");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Note",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Note",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Note_UserId",
                table: "Note",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Note_AEPUser_UserId",
                table: "Note",
                column: "UserId",
                principalTable: "AEPUser",
                principalColumn: "Id");
        }
    }
}
