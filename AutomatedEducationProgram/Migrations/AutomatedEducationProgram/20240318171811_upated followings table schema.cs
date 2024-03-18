using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutomatedEducationProgram.Migrations.AutomatedEducationProgram
{
    public partial class upatedfollowingstableschema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Followings_AEPUser_FollowedId",
                table: "Followings");

            migrationBuilder.DropForeignKey(
                name: "FK_Followings_AEPUser_FollowerId",
                table: "Followings");

            migrationBuilder.DropIndex(
                name: "IX_Followings_FollowedId",
                table: "Followings");

            migrationBuilder.DropIndex(
                name: "IX_Followings_FollowerId",
                table: "Followings");

            migrationBuilder.DropColumn(
                name: "FollowedId",
                table: "Followings");

            migrationBuilder.DropColumn(
                name: "FollowerId",
                table: "Followings");

            migrationBuilder.AddColumn<string>(
                name: "Followed",
                table: "Followings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Follower",
                table: "Followings",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Followed",
                table: "Followings");

            migrationBuilder.DropColumn(
                name: "Follower",
                table: "Followings");

            migrationBuilder.AddColumn<string>(
                name: "FollowedId",
                table: "Followings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FollowerId",
                table: "Followings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Followings_FollowedId",
                table: "Followings",
                column: "FollowedId");

            migrationBuilder.CreateIndex(
                name: "IX_Followings_FollowerId",
                table: "Followings",
                column: "FollowerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Followings_AEPUser_FollowedId",
                table: "Followings",
                column: "FollowedId",
                principalTable: "AEPUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Followings_AEPUser_FollowerId",
                table: "Followings",
                column: "FollowerId",
                principalTable: "AEPUser",
                principalColumn: "Id");
        }
    }
}
