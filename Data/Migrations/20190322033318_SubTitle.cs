using Microsoft.EntityFrameworkCore.Migrations;

namespace Waku.Migrations
{
    public partial class SubTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "BlogPosts",
                newName: "Thumbnail");

            migrationBuilder.AddColumn<string>(
                name: "SubTitle",
                table: "BlogPosts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTitle",
                table: "BlogPosts");

            migrationBuilder.RenameColumn(
                name: "Thumbnail",
                table: "BlogPosts",
                newName: "Image");
        }
    }
}
