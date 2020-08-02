﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TrainingTask.Infrastructure.Database;

namespace TrainingTask.Infrastructure.Migrations
{
    [DbContext(typeof(MyContext))]
    partial class MyContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("TrainingTask.Core.Entity.SubjectEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("AirplaneKey")
                        .HasColumnName("airplane_key");

                    b.Property<string>("AirplaneValue")
                        .HasColumnName("airplane_value");

                    b.Property<string>("ClassifyKey")
                        .HasColumnName("classify_key");

                    b.Property<string>("ClassifyValue")
                        .HasColumnName("classify_value");

                    b.Property<int>("Copy")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("copy")
                        .HasDefaultValue(0);

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("t_create");

                    b.Property<string>("CreatorName")
                        .HasColumnName("creator_name");

                    b.Property<sbyte?>("DeleteFlag")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("delete_flag")
                        .HasColumnType("tinyint(2)")
                        .HasDefaultValueSql("'0'");

                    b.Property<string>("Desc")
                        .HasColumnName("desc");

                    b.Property<string>("ExpectedResult")
                        .HasColumnName("expected_result");

                    b.Property<float>("FinishPercent")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("finish_percent")
                        .HasDefaultValue(0f);

                    b.Property<DateTime?>("ModifiedTime")
                        .ValueGeneratedOnUpdate()
                        .HasColumnName("t_modified");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<long>("OriginalId")
                        .HasColumnName("original_id");

                    b.Property<float>("PassPercent")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("pass_percent")
                        .HasDefaultValue(0f);

                    b.Property<string>("SubjectNumb")
                        .HasColumnName("subjectNumb");

                    b.Property<string>("TagDisplay")
                        .HasColumnName("tag_display");

                    b.Property<long>("TaskId")
                        .HasColumnName("task_id");

                    b.HasKey("Id");

                    b.ToTable("t_subject");
                });

            modelBuilder.Entity("TrainingTask.Core.Entity.SubjectScoreEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("AirplaneValue")
                        .HasColumnName("airplane_value");

                    b.Property<string>("ClassifyValue")
                        .HasColumnName("classify_value");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("t_create");

                    b.Property<sbyte?>("DeleteFlag")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("delete_flag")
                        .HasColumnType("tinyint(2)")
                        .HasDefaultValueSql("'0'");

                    b.Property<string>("Desc")
                        .HasColumnName("desc");

                    b.Property<DateTime?>("ModifiedTime")
                        .ValueGeneratedOnUpdate()
                        .HasColumnName("t_modified");

                    b.Property<int>("Result")
                        .HasColumnName("result");

                    b.Property<int>("Status")
                        .HasColumnName("status");

                    b.Property<long>("SubjectId")
                        .HasColumnName("subject_id");

                    b.Property<string>("SubjectName")
                        .HasColumnName("subject_name");

                    b.Property<string>("TagDisplay")
                        .HasColumnName("tag_display");

                    b.Property<long>("TaskId")
                        .HasColumnName("task_id");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("t_subject_score");
                });

            modelBuilder.Entity("TrainingTask.Core.Entity.SubjectTagRefEntity", b =>
                {
                    b.Property<long>("SubjectId")
                        .HasColumnName("subject_id");

                    b.Property<long>("TagId")
                        .HasColumnName("tag_id");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("t_create");

                    b.Property<sbyte?>("DeleteFlag")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("delete_flag")
                        .HasColumnType("tinyint(2)")
                        .HasDefaultValueSql("'0'");

                    b.Property<DateTime?>("ModifiedTime")
                        .ValueGeneratedOnUpdate()
                        .HasColumnName("t_modified");

                    b.HasKey("SubjectId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("t_subject_tag_ref");
                });

            modelBuilder.Entity("TrainingTask.Core.Entity.TagEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("t_create");

                    b.Property<sbyte?>("DeleteFlag")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("delete_flag")
                        .HasColumnType("tinyint(2)")
                        .HasDefaultValueSql("'0'");

                    b.Property<DateTime?>("ModifiedTime")
                        .ValueGeneratedOnUpdate()
                        .HasColumnName("t_modified");

                    b.Property<long>("OriginalId")
                        .HasColumnName("original_id");

                    b.Property<string>("TagName")
                        .HasColumnName("tag");

                    b.HasKey("Id");

                    b.ToTable("t_tag");
                });

            modelBuilder.Entity("TrainingTask.Core.Entity.TaskEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("AirplaneTypeKey")
                        .HasColumnName("airplane_type_key");

                    b.Property<string>("AirplaneTypeValue")
                        .HasColumnName("airplane_type_value");

                    b.Property<int?>("ClassHour")
                        .HasColumnName("class_hour");

                    b.Property<int>("Copy")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("copy")
                        .HasDefaultValue(0);

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("t_create");

                    b.Property<long?>("CreatorId")
                        .HasColumnName("creator_id");

                    b.Property<string>("CreatorName")
                        .HasColumnName("creator_name");

                    b.Property<sbyte?>("DeleteFlag")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("delete_flag")
                        .HasColumnType("tinyint(2)")
                        .HasDefaultValueSql("'0'");

                    b.Property<float>("DurationAvg")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("duration_avg")
                        .HasDefaultValue(0f);

                    b.Property<float>("FinishPercent")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("finish_percent")
                        .HasDefaultValue(0f);

                    b.Property<string>("LevelKey")
                        .HasColumnName("level_key");

                    b.Property<string>("LevelValue")
                        .HasColumnName("level_value");

                    b.Property<DateTime?>("ModifiedTime")
                        .ValueGeneratedOnUpdate()
                        .HasColumnName("t_modified");

                    b.Property<float>("PassPercent")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("pass_percent")
                        .HasDefaultValue(0f);

                    b.Property<string>("TagDisplay")
                        .HasColumnName("tag_display");

                    b.Property<string>("TaskDesc")
                        .HasColumnName("task_desc");

                    b.Property<string>("TaskName")
                        .HasColumnName("task_name");

                    b.Property<string>("TaskTypeKey")
                        .HasColumnName("task_type_key");

                    b.Property<string>("TaskTypeValue")
                        .HasColumnName("task_type_value");

                    b.Property<string>("TypeLevelKey")
                        .HasColumnName("type_level_key");

                    b.Property<string>("TypeLevelValue")
                        .HasColumnName("type_level_value");

                    b.HasKey("Id");

                    b.ToTable("t_task");
                });

            modelBuilder.Entity("TrainingTask.Core.Entity.TaskScoreEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("t_create");

                    b.Property<sbyte?>("DeleteFlag")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("delete_flag")
                        .HasColumnType("tinyint(2)")
                        .HasDefaultValueSql("'0'");

                    b.Property<string>("Department")
                        .HasColumnName("department");

                    b.Property<float>("Duration")
                        .HasColumnName("duration");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnName("end_time");

                    b.Property<DateTime?>("ModifiedTime")
                        .ValueGeneratedOnUpdate()
                        .HasColumnName("t_modified");

                    b.Property<long>("PlanId")
                        .HasColumnName("plan_id");

                    b.Property<int>("Result")
                        .HasColumnName("result");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnName("start_time");

                    b.Property<int>("Status")
                        .HasColumnName("status");

                    b.Property<long>("TaskId")
                        .HasColumnName("task_id");

                    b.Property<string>("TaskName")
                        .HasColumnName("task_name");

                    b.Property<long>("UserId")
                        .HasColumnName("userId");

                    b.Property<string>("UserName")
                        .HasColumnName("userName");

                    b.HasKey("Id");

                    b.ToTable("t_task_score");
                });

            modelBuilder.Entity("TrainingTask.Core.Entity.TaskSubjectRefEntity", b =>
                {
                    b.Property<long>("TaskId")
                        .HasColumnName("task_id");

                    b.Property<long>("SubjectId")
                        .HasColumnName("subject_id");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("t_create");

                    b.Property<sbyte?>("DeleteFlag")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("delete_flag")
                        .HasColumnType("tinyint(2)")
                        .HasDefaultValueSql("'0'");

                    b.Property<DateTime?>("ModifiedTime")
                        .ValueGeneratedOnUpdate()
                        .HasColumnName("t_modified");

                    b.HasKey("TaskId", "SubjectId");

                    b.HasIndex("SubjectId");

                    b.ToTable("t_task_subject_ref");
                });

            modelBuilder.Entity("TrainingTask.Core.Entity.SubjectScoreEntity", b =>
                {
                    b.HasOne("TrainingTask.Core.Entity.TaskScoreEntity", "TaskScore")
                        .WithMany("SubjectScores")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TrainingTask.Core.Entity.SubjectTagRefEntity", b =>
                {
                    b.HasOne("TrainingTask.Core.Entity.SubjectEntity", "Subject")
                        .WithMany("TagRefEntities")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TrainingTask.Core.Entity.TagEntity", "Tag")
                        .WithMany("SubjectEntities")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TrainingTask.Core.Entity.TaskSubjectRefEntity", b =>
                {
                    b.HasOne("TrainingTask.Core.Entity.SubjectEntity", "Subject")
                        .WithMany("TaskRefEntities")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TrainingTask.Core.Entity.TaskEntity", "Task")
                        .WithMany("SubjectRefEntities")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
