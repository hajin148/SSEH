using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutomatedEducationProgram.Migrations.AutomatedEducationProgram
{
    public partial class Addingattributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AEPUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Major = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelfDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AEPUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AEPUserAEPUser",
                columns: table => new
                {
                    followeesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    followersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AEPUserAEPUser", x => new { x.followeesId, x.followersId });
                    table.ForeignKey(
                        name: "FK_AEPUserAEPUser_AEPUser_followeesId",
                        column: x => x.followeesId,
                        principalTable: "AEPUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AEPUserAEPUser_AEPUser_followersId",
                        column: x => x.followersId,
                        principalTable: "AEPUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Note",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Note", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Note_AEPUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AEPUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExamQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentNoteId = table.Column<int>(type: "int", nullable: true),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnswerA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnswerB = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnswerC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnswerD = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamQuestion_Note_ParentNoteId",
                        column: x => x.ParentNoteId,
                        principalTable: "Note",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VocabularyWord",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Term = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Definition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentNoteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyWord", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VocabularyWord_Note_ParentNoteId",
                        column: x => x.ParentNoteId,
                        principalTable: "Note",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AEPUserAEPUser_followersId",
                table: "AEPUserAEPUser",
                column: "followersId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestion_ParentNoteId",
                table: "ExamQuestion",
                column: "ParentNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_UserId",
                table: "Note",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyWord_ParentNoteId",
                table: "VocabularyWord",
                column: "ParentNoteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AEPUserAEPUser");

            migrationBuilder.DropTable(
                name: "ExamQuestion");

            migrationBuilder.DropTable(
                name: "VocabularyWord");

            migrationBuilder.DropTable(
                name: "Note");

            migrationBuilder.DropTable(
                name: "AEPUser");
        }
    }
}
