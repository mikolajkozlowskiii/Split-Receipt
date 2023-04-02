using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Split_Receipt.Migrations
{
    /// <inheritdoc />
    public partial class AddedEmailToGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "User_Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "User_Groups");
        }
    }
}
