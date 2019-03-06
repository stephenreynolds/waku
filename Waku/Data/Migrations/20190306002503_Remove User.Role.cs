using Microsoft.EntityFrameworkCore.Migrations;

namespace Waku.Migrations
{
    public partial class RemoveUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");
        }
    }
}
