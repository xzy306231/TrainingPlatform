using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrainingTask.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_subject",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    delete_flag = table.Column<sbyte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    t_modified = table.Column<DateTime>(nullable: true),
                    task_id = table.Column<long>(nullable: false),
                    original_id = table.Column<long>(nullable: false),
                    subjectNumb = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    desc = table.Column<string>(nullable: true),
                    classify_key = table.Column<string>(nullable: true),
                    classify_value = table.Column<string>(nullable: true),
                    airplane_key = table.Column<string>(nullable: true),
                    airplane_value = table.Column<string>(nullable: true),
                    expected_result = table.Column<string>(nullable: true),
                    tag_display = table.Column<string>(nullable: true),
                    creator_name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_subject", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_tag",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    delete_flag = table.Column<sbyte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    t_modified = table.Column<DateTime>(nullable: true),
                    original_id = table.Column<long>(nullable: false),
                    tag = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_tag", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_task",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    delete_flag = table.Column<sbyte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    t_modified = table.Column<DateTime>(nullable: true),
                    task_name = table.Column<string>(nullable: true),
                    task_desc = table.Column<string>(nullable: true),
                    task_type_key = table.Column<string>(nullable: true),
                    task_type_value = table.Column<string>(nullable: true),
                    type_level_key = table.Column<string>(nullable: true),
                    type_level_value = table.Column<string>(nullable: true),
                    level_key = table.Column<string>(nullable: true),
                    level_value = table.Column<string>(nullable: true),
                    airplane_type_key = table.Column<string>(nullable: true),
                    airplane_type_value = table.Column<string>(nullable: true),
                    class_hour = table.Column<int>(nullable: true),
                    creator_id = table.Column<long>(nullable: true),
                    creator_name = table.Column<string>(nullable: true),
                    tag_display = table.Column<string>(nullable: true),
                    copy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_task", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_task_score",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    delete_flag = table.Column<sbyte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    t_modified = table.Column<DateTime>(nullable: true),
                    userId = table.Column<long>(nullable: false),
                    userName = table.Column<string>(nullable: true),
                    department = table.Column<string>(nullable: true),
                    plan_id = table.Column<long>(nullable: false),
                    task_id = table.Column<long>(nullable: false),
                    task_name = table.Column<string>(nullable: true),
                    result = table.Column<int>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    start_time = table.Column<DateTime>(nullable: true),
                    end_time = table.Column<DateTime>(nullable: true),
                    duration = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_task_score", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_subject_tag_ref",
                columns: table => new
                {
                    subject_id = table.Column<long>(nullable: false),
                    tag_id = table.Column<long>(nullable: false),
                    delete_flag = table.Column<sbyte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    t_modified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_subject_tag_ref", x => new { x.subject_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK_t_subject_tag_ref_t_subject_subject_id",
                        column: x => x.subject_id,
                        principalTable: "t_subject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_subject_tag_ref_t_tag_tag_id",
                        column: x => x.tag_id,
                        principalTable: "t_tag",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_task_subject_ref",
                columns: table => new
                {
                    task_id = table.Column<long>(nullable: false),
                    subject_id = table.Column<long>(nullable: false),
                    delete_flag = table.Column<sbyte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    t_modified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_task_subject_ref", x => new { x.task_id, x.subject_id });
                    table.ForeignKey(
                        name: "FK_t_task_subject_ref_t_subject_subject_id",
                        column: x => x.subject_id,
                        principalTable: "t_subject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_task_subject_ref_t_task_task_id",
                        column: x => x.task_id,
                        principalTable: "t_task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_subject_score",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    delete_flag = table.Column<sbyte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    t_modified = table.Column<DateTime>(nullable: true),
                    task_id = table.Column<long>(nullable: false),
                    subject_id = table.Column<long>(nullable: false),
                    subject_name = table.Column<string>(nullable: true),
                    desc = table.Column<string>(nullable: true),
                    tag_display = table.Column<string>(nullable: true),
                    airplane_value = table.Column<string>(nullable: true),
                    classify_value = table.Column<string>(nullable: true),
                    result = table.Column<int>(nullable: false),
                    status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_subject_score", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_subject_score_t_task_score_task_id",
                        column: x => x.task_id,
                        principalTable: "t_task_score",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_subject_score_task_id",
                table: "t_subject_score",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_subject_tag_ref_tag_id",
                table: "t_subject_tag_ref",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_task_subject_ref_subject_id",
                table: "t_task_subject_ref",
                column: "subject_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_subject_score");

            migrationBuilder.DropTable(
                name: "t_subject_tag_ref");

            migrationBuilder.DropTable(
                name: "t_task_subject_ref");

            migrationBuilder.DropTable(
                name: "t_task_score");

            migrationBuilder.DropTable(
                name: "t_tag");

            migrationBuilder.DropTable(
                name: "t_subject");

            migrationBuilder.DropTable(
                name: "t_task");
        }
    }
}
