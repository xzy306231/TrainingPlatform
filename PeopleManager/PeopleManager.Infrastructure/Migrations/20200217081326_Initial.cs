using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PeopleManager.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_person_info",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    delete_flag = table.Column<byte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false),
                    t_modified = table.Column<DateTime>(nullable: false),
                    original_id = table.Column<long>(type: "bigint(20)", nullable: false),
                    teacher_flag = table.Column<byte>(type: "tinyint(2)", nullable: false, defaultValueSql: "'0'"),
                    student_flag = table.Column<byte>(type: "tinyint(2)", nullable: false, defaultValueSql: "'0'"),
                    user_name = table.Column<string>(nullable: false),
                    user_number = table.Column<string>(nullable: false),
                    gender = table.Column<string>(nullable: true),
                    birthday = table.Column<DateTime>(nullable: true),
                    education_key = table.Column<string>(nullable: true),
                    education_value = table.Column<string>(nullable: true),
                    school_tag = table.Column<string>(nullable: true),
                    house_address = table.Column<string>(nullable: true),
                    regular_address = table.Column<string>(nullable: true),
                    user_phone = table.Column<string>(nullable: false),
                    nationality = table.Column<string>(nullable: true),
                    nation = table.Column<string>(nullable: true),
                    blood_type = table.Column<string>(nullable: true),
                    native_place = table.Column<string>(nullable: true),
                    marriage_status = table.Column<string>(nullable: true),
                    state_of_health = table.Column<string>(nullable: true),
                    user_email = table.Column<string>(nullable: true),
                    employment_date = table.Column<DateTime>(nullable: true),
                    photo_path = table.Column<string>(nullable: true),
                    sec_level = table.Column<byte>(type: "tinyint(2)", nullable: true),
                    qualification_name = table.Column<string>(nullable: true),
                    qualification_type_key = table.Column<string>(nullable: true),
                    qualification_type_value = table.Column<string>(nullable: true),
                    qualification_get_date = table.Column<DateTime>(nullable: true),
                    qualification_expiration_date = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_person_info", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_certificate_info",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    delete_flag = table.Column<byte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false),
                    t_modified = table.Column<DateTime>(nullable: false),
                    person_id = table.Column<long>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    code = table.Column<string>(nullable: true),
                    type_key = table.Column<string>(nullable: true),
                    airplane_model_key = table.Column<string>(nullable: true),
                    airplane_model_value = table.Column<string>(nullable: true),
                    type_value = table.Column<string>(nullable: true),
                    valid_key = table.Column<string>(nullable: true),
                    valid_value = table.Column<string>(nullable: true),
                    pass_date = table.Column<DateTime>(nullable: true),
                    get_date = table.Column<DateTime>(nullable: true),
                    last_endorse_date = table.Column<DateTime>(nullable: true),
                    expiration_date = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_certificate_info", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_certificate_info_t_person_info_person_id",
                        column: x => x.person_id,
                        principalTable: "t_person_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_license_info",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    delete_flag = table.Column<byte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false),
                    t_modified = table.Column<DateTime>(nullable: false),
                    person_id = table.Column<long>(nullable: false),
                    license_name = table.Column<string>(nullable: true),
                    valid_key = table.Column<string>(nullable: true),
                    valid_value = table.Column<string>(nullable: true),
                    start_date = table.Column<DateTime>(nullable: true),
                    end_date = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_license_info", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_license_info_t_person_info_person_id",
                        column: x => x.person_id,
                        principalTable: "t_person_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_rewards_and_punishment",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    delete_flag = table.Column<byte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false),
                    t_modified = table.Column<DateTime>(nullable: false),
                    person_id = table.Column<long>(nullable: false),
                    event_name = table.Column<string>(nullable: true),
                    event_type_key = table.Column<string>(nullable: true),
                    event_type_value = table.Column<string>(nullable: true),
                    event_date = table.Column<DateTime>(nullable: true),
                    event_result = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_rewards_and_punishment", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_rewards_and_punishment_t_person_info_person_id",
                        column: x => x.person_id,
                        principalTable: "t_person_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_training_record",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    delete_flag = table.Column<byte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false),
                    t_modified = table.Column<DateTime>(nullable: false),
                    person_id = table.Column<long>(nullable: false),
                    training_date = table.Column<DateTime>(nullable: true),
                    project_name = table.Column<string>(nullable: true),
                    content = table.Column<string>(nullable: true),
                    status_key = table.Column<string>(nullable: true),
                    status_value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_training_record", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_training_record_t_person_info_person_id",
                        column: x => x.person_id,
                        principalTable: "t_person_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_work_info",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    delete_flag = table.Column<byte>(type: "tinyint(2)", nullable: true, defaultValueSql: "'0'"),
                    t_create = table.Column<DateTime>(nullable: false),
                    t_modified = table.Column<DateTime>(nullable: false),
                    person_id = table.Column<long>(nullable: false),
                    department_key = table.Column<string>(nullable: true),
                    department_value = table.Column<string>(nullable: true),
                    teacher_type_key = table.Column<string>(nullable: true),
                    teacher_type_value = table.Column<string>(nullable: true),
                    airplane_model_key = table.Column<string>(nullable: true),
                    airplane_model_value = table.Column<string>(nullable: true),
                    base_key = table.Column<string>(nullable: true),
                    base_value = table.Column<string>(nullable: true),
                    hire_date = table.Column<DateTime>(nullable: true),
                    skill_level_key = table.Column<string>(nullable: true),
                    skill_level_value = table.Column<string>(nullable: true),
                    fly_status_key = table.Column<string>(nullable: true),
                    fly_status_value = table.Column<string>(nullable: true),
                    total_duration = table.Column<double>(nullable: false),
                    training_duration = table.Column<double>(nullable: false),
                    actual_flight_number = table.Column<int>(nullable: false),
                    actual_duration = table.Column<double>(nullable: false),
                    current_actual_duration = table.Column<double>(nullable: false),
                    current_flight_number = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_work_info", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_work_info_t_person_info_person_id",
                        column: x => x.person_id,
                        principalTable: "t_person_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_certificate_info_person_id",
                table: "t_certificate_info",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_license_info_person_id",
                table: "t_license_info",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_rewards_and_punishment_person_id",
                table: "t_rewards_and_punishment",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_training_record_person_id",
                table: "t_training_record",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_work_info_person_id",
                table: "t_work_info",
                column: "person_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_certificate_info");

            migrationBuilder.DropTable(
                name: "t_license_info");

            migrationBuilder.DropTable(
                name: "t_rewards_and_punishment");

            migrationBuilder.DropTable(
                name: "t_training_record");

            migrationBuilder.DropTable(
                name: "t_work_info");

            migrationBuilder.DropTable(
                name: "t_person_info");
        }
    }
}
