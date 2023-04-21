using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComputerCourses.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Teachers_TeacherId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Ended",
                table: "Descriptions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Courses");

            migrationBuilder.AlterColumn<List<string>>(
                name: "Technologies",
                table: "Courses",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxStudents",
                table: "Courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StudyStart",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "CourseTeacher",
                columns: table => new
                {
                    CoursesId = table.Column<int>(type: "integer", nullable: false),
                    TeachersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTeacher", x => new { x.CoursesId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_CourseTeacher_Courses_CoursesId",
                        column: x => x.CoursesId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseTeacher_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeacher_TeachersId",
                table: "CourseTeacher",
                column: "TeachersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseTeacher");

            migrationBuilder.DropColumn(
                name: "MaxStudents",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "StudyStart",
                table: "Courses");

            migrationBuilder.AddColumn<bool>(
                name: "Ended",
                table: "Descriptions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<List<string>>(
                name: "Technologies",
                table: "Courses",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Courses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "Courses",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Teachers_TeacherId",
                table: "Courses",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }
    }
}
