using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutomatedEducationProgram.Migrations.AutomatedEducationProgram
{
    public partial class revsingdocTextrelationshipsaddingpublicfieldtonotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelevantDoc",
                table: "VocabularyWord");

            migrationBuilder.DropColumn(
                name: "RelevantDoc",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DocumentTexts");

            migrationBuilder.AddColumn<int>(
                name: "RelevantDocId",
                table: "VocabularyWord",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Note",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RelevantDocId",
                table: "ExamQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "parentNoteId",
                table: "DocumentTexts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyWord_RelevantDocId",
                table: "VocabularyWord",
                column: "RelevantDocId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestions_RelevantDocId",
                table: "ExamQuestions",
                column: "RelevantDocId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTexts_parentNoteId",
                table: "DocumentTexts",
                column: "parentNoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTexts_Note_parentNoteId",
                table: "DocumentTexts",
                column: "parentNoteId",
                principalTable: "Note",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamQuestions_DocumentTexts_RelevantDocId",
                table: "ExamQuestions",
                column: "RelevantDocId",
                principalTable: "DocumentTexts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VocabularyWord_DocumentTexts_RelevantDocId",
                table: "VocabularyWord",
                column: "RelevantDocId",
                principalTable: "DocumentTexts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTexts_Note_parentNoteId",
                table: "DocumentTexts");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamQuestions_DocumentTexts_RelevantDocId",
                table: "ExamQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_VocabularyWord_DocumentTexts_RelevantDocId",
                table: "VocabularyWord");

            migrationBuilder.DropIndex(
                name: "IX_VocabularyWord_RelevantDocId",
                table: "VocabularyWord");

            migrationBuilder.DropIndex(
                name: "IX_ExamQuestions_RelevantDocId",
                table: "ExamQuestions");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTexts_parentNoteId",
                table: "DocumentTexts");

            migrationBuilder.DropColumn(
                name: "RelevantDocId",
                table: "VocabularyWord");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "RelevantDocId",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "parentNoteId",
                table: "DocumentTexts");

            migrationBuilder.AddColumn<int>(
                name: "RelevantDoc",
                table: "VocabularyWord",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RelevantDoc",
                table: "ExamQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DocumentTexts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
