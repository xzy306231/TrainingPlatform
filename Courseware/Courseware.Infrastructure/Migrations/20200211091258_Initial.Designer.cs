﻿// <auto-generated />
using System;
using Courseware.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Courseware.Infrastructure.Migrations
{
    [DbContext(typeof(MyContext))]
    [Migration("20200211091258_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("Courseware.Core.Entities.KnowledgeTagEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("t_create")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<byte?>("DeleteFlag")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("delete_flag")
                        .HasColumnType("tinyint(2)")
                        .HasDefaultValueSql("'0'");

                    b.Property<DateTime>("ModifiedTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("t_modified")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<long>("OriginalId")
                        .HasColumnName("original_id")
                        .HasColumnType("bigint(20)");

                    b.Property<string>("TagName")
                        .IsRequired()
                        .HasColumnName("tag");

                    b.HasKey("Id");

                    b.ToTable("t_knowledge_tag");
                });

            modelBuilder.Entity("Courseware.Core.Entities.ResourceEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime?>("CheckDate")
                        .HasColumnName("check_date");

                    b.Property<string>("CheckRemark")
                        .HasColumnName("check_remark");

                    b.Property<string>("CheckStatus")
                        .HasColumnName("check_status");

                    b.Property<long?>("CheckerId")
                        .HasColumnName("checker_id")
                        .HasColumnType("bigint(20)");

                    b.Property<string>("CheckerName")
                        .HasColumnName("checker_name");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("t_create")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<long>("CreatorId")
                        .HasColumnName("creator_id");

                    b.Property<string>("CreatorName")
                        .HasColumnName("creator_name");

                    b.Property<byte?>("DeleteFlag")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("delete_flag")
                        .HasColumnType("tinyint(2)")
                        .HasDefaultValueSql("'0'");

                    b.Property<long>("FileSize")
                        .HasColumnName("file_size");

                    b.Property<string>("FileSizeDisplay")
                        .HasColumnName("file_size_display");

                    b.Property<string>("FileSuffix")
                        .HasColumnName("file_suffix");

                    b.Property<string>("GroupName")
                        .HasColumnName("group_name");

                    b.Property<string>("MD5Code")
                        .HasColumnName("md5_code");

                    b.Property<DateTime>("ModifiedTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("t_modified")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("OriginalUrl")
                        .HasColumnName("original_url");

                    b.Property<string>("PathToFolder")
                        .HasColumnName("path_to_folder");

                    b.Property<string>("PathToIndex")
                        .HasColumnName("path_to_index");

                    b.Property<string>("ResourceDesc")
                        .HasColumnName("resource_desc");

                    b.Property<int?>("ResourceDuration")
                        .HasColumnName("resource_duration");

                    b.Property<string>("ResourceLevel")
                        .HasColumnName("resource_level");

                    b.Property<string>("ResourceName")
                        .IsRequired()
                        .HasColumnName("resource_name");

                    b.Property<string>("ResourceTagsDisplay")
                        .HasColumnName("resource_tags_display");

                    b.Property<string>("ResourceType")
                        .HasColumnName("resource_type");

                    b.Property<string>("SCORMVersion")
                        .HasColumnName("SCORM_version");

                    b.Property<string>("ThumbnailPath")
                        .HasColumnName("thumbnail_path");

                    b.Property<string>("TitleFromManifest")
                        .HasColumnName("title_from_manifest");

                    b.Property<string>("TransfType")
                        .HasColumnName("transf_type");

                    b.Property<string>("TransformUrl")
                        .HasColumnName("transform_url");

                    b.HasKey("Id");

                    b.ToTable("t_course_resource");
                });

            modelBuilder.Entity("Courseware.Core.Entities.ResourceTagEntity", b =>
                {
                    b.Property<long>("ResourceId")
                        .HasColumnName("resource_id");

                    b.Property<long>("TagId")
                        .HasColumnName("tag_id");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("t_create")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<byte?>("DeleteFlag")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("delete_flag")
                        .HasColumnType("tinyint(2)")
                        .HasDefaultValueSql("'0'");

                    b.Property<DateTime>("ModifiedTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("t_modified")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("ResourceId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("t_resource_tag_ref");
                });

            modelBuilder.Entity("Courseware.Core.Entities.ResourceTagEntity", b =>
                {
                    b.HasOne("Courseware.Core.Entities.ResourceEntity", "Resource")
                        .WithMany("ResourceTags")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Courseware.Core.Entities.KnowledgeTagEntity", "Tag")
                        .WithMany("ResourceTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}