using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Split_Receipt.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUserEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Groups_AspNetUsers_UserEmail",
                table: "User_Groups");

            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "User_Groups",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_User_Groups_UserEmail",
                table: "User_Groups",
                newName: "IX_User_Groups_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Groups_AspNetUsers_UserId",
                table: "User_Groups",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Groups_AspNetUsers_UserId",
                table: "User_Groups");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "User_Groups",
                newName: "UserEmail");

            migrationBuilder.RenameIndex(
                name: "IX_User_Groups_UserId",
                table: "User_Groups",
                newName: "IX_User_Groups_UserEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Groups_AspNetUsers_UserEmail",
                table: "User_Groups",
                column: "UserEmail",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
