using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutomatedEducationProgram.Migrations.AutomatedEducationProgram
{
    public partial class UpdatedExamQModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Answer",
                table: "ExamQuestions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Answer",
                table: "ExamQuestions");
        }
    }
}
