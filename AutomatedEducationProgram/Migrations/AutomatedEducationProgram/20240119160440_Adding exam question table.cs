using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutomatedEducationProgram.Migrations.AutomatedEducationProgram
{
    public partial class Addingexamquestiontable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamQuestion_Note_ParentNoteId",
                table: "ExamQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExamQuestion",
                table: "ExamQuestion");

            migrationBuilder.RenameTable(
                name: "ExamQuestion",
                newName: "ExamQuestions");

            migrationBuilder.RenameIndex(
                name: "IX_ExamQuestion_ParentNoteId",
                table: "ExamQuestions",
                newName: "IX_ExamQuestions_ParentNoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExamQuestions",
                table: "ExamQuestions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamQuestions_Note_ParentNoteId",
                table: "ExamQuestions",
                column: "ParentNoteId",
                principalTable: "Note",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamQuestions_Note_ParentNoteId",
                table: "ExamQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExamQuestions",
                table: "ExamQuestions");

            migrationBuilder.RenameTable(
                name: "ExamQuestions",
                newName: "ExamQuestion");

            migrationBuilder.RenameIndex(
                name: "IX_ExamQuestions_ParentNoteId",
                table: "ExamQuestion",
                newName: "IX_ExamQuestion_ParentNoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExamQuestion",
                table: "ExamQuestion",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamQuestion_Note_ParentNoteId",
                table: "ExamQuestion",
                column: "ParentNoteId",
                principalTable: "Note",
                principalColumn: "Id");
        }
    }
}
