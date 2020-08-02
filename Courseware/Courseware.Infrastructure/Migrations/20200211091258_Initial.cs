using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Courseware.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_course_resource",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    delete_flag = table.Column<byte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    t_modified = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    creator_id = table.Column<long>(nullable: false),
                    creator_name = table.Column<string>(nullable: true),
                    resource_name = table.Column<string>(nullable: false),
                    resource_desc = table.Column<string>(nullable: true),
                    resource_type = table.Column<string>(nullable: true),
                    resource_duration = table.Column<int>(nullable: true),
                    resource_level = table.Column<string>(nullable: true),
                    transf_type = table.Column<string>(nullable: true),
                    thumbnail_path = table.Column<string>(nullable: true),
                    md5_code = table.Column<string>(nullable: true),
                    file_suffix = table.Column<string>(nullable: true),
                    original_url = table.Column<string>(nullable: true),
                    transform_url = table.Column<string>(nullable: true),
                    file_size = table.Column<long>(nullable: false),
                    file_size_display = table.Column<string>(nullable: true),
                    group_name = table.Column<string>(nullable: true),
                    title_from_manifest = table.Column<string>(nullable: true),
                    path_to_index = table.Column<string>(nullable: true),
                    path_to_folder = table.Column<string>(nullable: true),
                    SCORM_version = table.Column<string>(nullable: true),
                    checker_id = table.Column<long>(type: "bigint(20)", nullable: true),
                    checker_name = table.Column<string>(nullable: true),
                    check_date = table.Column<DateTime>(nullable: true),
                    check_remark = table.Column<string>(nullable: true),
                    check_status = table.Column<string>(nullable: true),
                    resource_tags_display = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_course_resource", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_knowledge_tag",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    delete_flag = table.Column<byte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    t_modified = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    original_id = table.Column<long>(type: "bigint(20)", nullable: false),
                    tag = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_knowledge_tag", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_resource_tag_ref",
                columns: table => new
                {
                    resource_id = table.Column<long>(nullable: false),
                    tag_id = table.Column<long>(nullable: false),
                    delete_flag = table.Column<byte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    t_modified = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_resource_tag_ref", x => new { x.resource_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK_t_resource_tag_ref_t_course_resource_resource_id",
                        column: x => x.resource_id,
                        principalTable: "t_course_resource",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_resource_tag_ref_t_knowledge_tag_tag_id",
                        column: x => x.tag_id,
                        principalTable: "t_knowledge_tag",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_resource_tag_ref_tag_id",
                table: "t_resource_tag_ref",
                column: "tag_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_resource_tag_ref");

            migrationBuilder.DropTable(
                name: "t_course_resource");

            migrationBuilder.DropTable(
                name: "t_knowledge_tag");
        }
    }
}
