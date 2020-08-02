using System;
using System.IO;
using Course.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;


public partial class pf_course_manage_v1Context : DbContext
{
    public pf_course_manage_v1Context()
    {
    }

    public pf_course_manage_v1Context(DbContextOptions<pf_course_manage_v1Context> options)
        : base(options)
    {
    }

    public static string ConnectionString => GetConnectionString();

    private static string GetConnectionString()
    {
        var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(new pf_course_manage_v1Context().GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
        var config = builder.Build();
        return config.GetConnectionString("conn");
    }
    public virtual DbSet<t_course> t_course { get; set; }
    public virtual DbSet<t_course_know_tag> t_course_know_tag { get; set; }
    public virtual DbSet<t_course_resource> t_course_resource { get; set; }
    public virtual DbSet<t_course_struct> t_course_struct { get; set; }
    public virtual DbSet<t_knowledge_tag> t_knowledge_tag { get; set; }
    public virtual DbSet<t_program_course_ref> t_program_course_ref { get; set; }
    public virtual DbSet<t_struct_resource> t_struct_resource { get; set; }
    public virtual DbSet<t_training_program> t_training_program { get; set; }
    public virtual DbSet<t_courseware_page_bus> t_courseware_page_bus { get; set; }
    public virtual DbSet<t_subject> t_subject { get; set; }
    public virtual DbSet<t_training_task> t_training_task { get; set; }
    public virtual DbSet<t_program_task_ref> t_program_task_ref { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(this.GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
            var config = builder.Build();
            string strConn = config.GetConnectionString("conn");
            optionsBuilder.UseMySql(strConn);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<t_courseware_page_bus>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.courseware_resource_id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.page_script).HasColumnType("longtext");

            entity.Property(e => e.page_sort).HasColumnType("int(11)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_subject>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.expect_result).HasColumnType("longtext");

            entity.Property(e => e.subject_desc).HasColumnType("longtext");

            entity.Property(e => e.plane_type).HasColumnType("varchar(10)");

            entity.Property(e => e.subject_kind).HasColumnType("varchar(10)");

            entity.Property(e => e.subject_name).HasColumnType("varchar(200)");

            entity.Property(e => e.subject_number).HasColumnType("varchar(100)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.task_id).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<t_program_task_ref>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.programid).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.taskid).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<t_course>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.approval_date).HasColumnType("datetime");

            entity.Property(e => e.approval_remarks).HasColumnType("longtext");

            entity.Property(e => e.approval_status).HasColumnType("varchar(5)");

            entity.Property(e => e.approval_user_id).HasColumnType("bigint(20)");

            entity.Property(e => e.approval_user_name).HasColumnType("varchar(50)");

            entity.Property(e => e.approval_user_number).HasColumnType("varchar(50)");

            entity.Property(e => e.course_confidential).HasColumnType("varchar(5)");

            entity.Property(e => e.course_count)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.course_desc).HasColumnType("longtext");

            entity.Property(e => e.course_name).HasColumnType("varchar(50)");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time).HasColumnType("datetime");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.learning_time)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.publish_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.thumbnail_path).HasColumnType("varchar(200)");

            entity.Property(e => e.update_by).HasColumnType("bigint(20)");

            entity.Property(e => e.update_time).HasColumnType("datetime");

            entity.Property(e => e.user_name).HasColumnType("varchar(50)");

            entity.Property(e => e.user_number).HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<t_course_know_tag>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_id).HasColumnType("bigint(20)");

            entity.Property(e => e.tag_id).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<t_course_resource>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time).HasColumnType("datetime");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.group_name).HasColumnType("varchar(50)");

            entity.Property(e => e.resource_confidential).HasColumnType("varchar(5)");

            entity.Property(e => e.resource_desc).HasColumnType("longtext");

            entity.Property(e => e.resource_extension).HasColumnType("varchar(20)");

            entity.Property(e => e.resource_name).HasColumnType("varchar(100)");

            entity.Property(e => e.resource_time).HasColumnType("int(11)");

            entity.Property(e => e.resource_type).HasColumnType("varchar(5)");

            entity.Property(e => e.resource_url).HasColumnType("varchar(200)");

            entity.Property(e => e.src_id).HasColumnType("bigint(20)");

            entity.Property(e => e.update_by).HasColumnType("bigint(20)");

            entity.Property(e => e.update_time).HasColumnType("datetime");
        });

        modelBuilder.Entity<t_course_struct>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_id).HasColumnType("bigint(20)");


            entity.Property(e => e.course_node_name).HasColumnType("varchar(100)");

            entity.Property(e => e.create_name).HasColumnType("varchar(20)");

            entity.Property(e => e.create_time).HasColumnType("datetime")
            .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'"); ;

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.node_sort).HasColumnType("int(11)");

            entity.Property(e => e.node_type).HasColumnType("varchar(20)");

            entity.Property(e => e.parent_id).HasColumnType("bigint(20)");

            entity.Property(e => e.resource_count).HasColumnType("int(11)");

            entity.Property(e => e.update_time).HasColumnType("datetime")
               .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_knowledge_tag>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");


            entity.Property(e => e.create_time)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.src_id).HasColumnType("bigint(20)");

            entity.Property(e => e.tag).HasColumnType("varchar(50)");

            entity.Property(e => e.update_time)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_program_course_ref>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.courseid).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.programid).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                               .HasColumnType("datetime")
                               .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_struct_resource>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_resouce_id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_struct_id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                 .HasColumnType("tinyint(2)")
                 .HasDefaultValueSql("'0'");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_training_program>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.content_question).HasColumnType("longtext");

            entity.Property(e => e.content_visible_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.course_visible_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.create_time)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.createby).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.endorsement_type).HasColumnType("varchar(20)");

            entity.Property(e => e.enter_visible_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.equipment).HasColumnType("longtext");

            entity.Property(e => e.equipment_visible_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.other_remark).HasColumnType("longtext");

            entity.Property(e => e.plane_type).HasColumnType("varchar(5)");

            entity.Property(e => e.plane_type1).HasColumnType("varchar(5)");

            entity.Property(e => e.purpose_visible_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.qualification_type).HasColumnType("varchar(20)");

            entity.Property(e => e.qualification_visible_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.range_purpose).HasColumnType("longtext");

            entity.Property(e => e.request_visible_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.standard_request).HasColumnType("longtext");

            entity.Property(e => e.sum_duration).HasColumnType("int(11)");

            entity.Property(e => e.technical_grade).HasColumnType("varchar(5)");

            entity.Property(e => e.train_program_name).HasColumnType("varchar(300)");

            entity.Property(e => e.train_time).HasColumnType("int(11)");

            entity.Property(e => e.train_type).HasColumnType("varchar(20)");

            entity.Property(e => e.trainsubject_visible_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.up_down_times).HasColumnType("int(11)");

            entity.Property(e => e.update_name).HasColumnType("varchar(50)");

            entity.Property(e => e.update_time)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                   .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.updateby).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<t_training_task>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_count).HasColumnType("int(11)");

            entity.Property(e => e.create_number).HasColumnType("varchar(50)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.knowledge_tag).HasColumnType("longtext");

            entity.Property(e => e.src_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.task_desc).HasColumnType("longtext");

            entity.Property(e => e.task_name).HasColumnType("varchar(200)");

            entity.Property(e => e.task_type).HasColumnType("varchar(50)");

            entity.Property(e => e.type_level).HasColumnType("varchar(50)");

            entity.Property(e => e.level).HasColumnType("varchar(50)");

            entity.Property(e => e.airplane_type).HasColumnType("varchar(50)");
        });
    }
}

