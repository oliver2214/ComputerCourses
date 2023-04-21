using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComputerCourses.Migrations
{
    public partial class v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Courses");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Clients",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Clients");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Courses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
