using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Split_Receipt.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUserIdFromUserGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Groups_AspNetUsers_UserId",
                table: "User_Groups");

            migrationBuilder.DropIndex(
                name: "IX_User_Groups_UserId",
                table: "User_Groups");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "User_Groups");

            migrationBuilder.AlterColumn<string>(
                name: "UserEmail",
                table: "User_Groups",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_User_Groups_UserEmail",
                table: "User_Groups",
                column: "UserEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Groups_AspNetUsers_UserEmail",
                table: "User_Groups",
                column: "UserEmail",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Groups_AspNetUsers_UserEmail",
                table: "User_Groups");

            migrationBuilder.DropIndex(
                name: "IX_User_Groups_UserEmail",
                table: "User_Groups");

            migrationBuilder.AlterColumn<string>(
                name: "UserEmail",
                table: "User_Groups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "User_Groups",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_User_Groups_UserId",
                table: "User_Groups",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Groups_AspNetUsers_UserId",
                table: "User_Groups",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
