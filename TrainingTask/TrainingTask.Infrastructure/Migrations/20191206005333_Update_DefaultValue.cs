using Microsoft.EntityFrameworkCore.Migrations;

namespace TrainingTask.Infrastructure.Migrations
{
    public partial class Update_DefaultValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "duration_avg",
                table: "t_task",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "finish_percent",
                table: "t_task",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "pass_percent",
                table: "t_task",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "finish_percent",
                table: "t_subject",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "pass_percent",
                table: "t_subject",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "duration_avg",
                table: "t_task");

            migrationBuilder.DropColumn(
                name: "finish_percent",
                table: "t_task");

            migrationBuilder.DropColumn(
                name: "pass_percent",
                table: "t_task");

            migrationBuilder.DropColumn(
                name: "finish_percent",
                table: "t_subject");

            migrationBuilder.DropColumn(
                name: "pass_percent",
                table: "t_subject");
        }
    }
}
