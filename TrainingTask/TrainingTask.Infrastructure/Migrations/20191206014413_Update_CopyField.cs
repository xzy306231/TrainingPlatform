using Microsoft.EntityFrameworkCore.Migrations;

namespace TrainingTask.Infrastructure.Migrations
{
    public partial class Update_CopyField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "copy",
                table: "t_task",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "copy",
                table: "t_subject",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "copy",
                table: "t_subject");

            migrationBuilder.AlterColumn<int>(
                name: "copy",
                table: "t_task",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: 0);
        }
    }
}
