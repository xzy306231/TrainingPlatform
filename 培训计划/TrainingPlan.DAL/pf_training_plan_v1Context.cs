using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


public partial class pf_training_plan_v1Context : DbContext
{
    public pf_training_plan_v1Context()
    {
    }

    public pf_training_plan_v1Context(DbContextOptions<pf_training_plan_v1Context> options)
        : base(options)
    {
    }

    public static string ConnectionString => GetConnectionString();

    private static string GetConnectionString()
    {
        var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(new pf_training_plan_v1Context().GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
        var config = builder.Build();
        return config.GetConnectionString("conn");
    }
    public virtual DbSet<t_course> t_course { get; set; }
    public virtual DbSet<t_config_learning_condition> t_config_learning_condition { get; set; }
    public virtual DbSet<t_course_know_tag> t_course_know_tag { get; set; }
    public virtual DbSet<t_course_resource> t_course_resource { get; set; }
    public virtual DbSet<t_course_struct> t_course_struct { get; set; }
    public virtual DbSet<t_knowledge_tag> t_knowledge_tag { get; set; }
    public virtual DbSet<t_program_course_ref> t_program_course_ref { get; set; }
    public virtual DbSet<t_plancourse_node_statistic> t_plancourse_node_statistic { get; set; }
    public virtual DbSet<t_plan_course_task_exam_ref> t_plan_course_task_exam_ref { get; set; }
    public virtual DbSet<t_program_subject_ref> t_program_subject_ref { get; set; }
    public virtual DbSet<t_resource_know_tag> t_resource_know_tag { get; set; }
    public virtual DbSet<t_struct_resource> t_struct_resource { get; set; }
    public virtual DbSet<t_training_program> t_training_program { get; set; }
    public virtual DbSet<t_train_subject> t_train_subject { get; set; }
    public virtual DbSet<t_training_plan> t_training_plan { get; set; }
    public virtual DbSet<t_trainingplan_stu> t_trainingplan_stu { get; set; }
    public virtual DbSet<t_subject_know_tag> t_subject_know_tag { get; set; }
    public virtual DbSet<t_learning_record> t_learning_record { get; set; }
    public virtual DbSet<t_courseware_page_bus> t_courseware_page_bus { get; set; }
    public virtual DbSet<t_subject_bus> t_subject_bus { get; set; }
    public virtual DbSet<t_task_bus> t_task_bus { get; set; }
    public virtual DbSet<t_course_node_learning_status> t_course_node_learning_status { get; set; }
    public virtual DbSet<t_course_node_learning_log> t_course_node_learning_log { get; set; }
    public virtual DbSet<t_training_task> t_training_task { get; set; }
    public virtual DbSet<t_task_bus_score> t_task_bus_score { get; set; }
    public virtual DbSet<t_examination_manage> t_examination_manage { get; set; }
    public virtual DbSet<t_trainingplan_stustatistic> t_trainingplan_stustatistic { get; set; }
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

        modelBuilder.Entity<t_config_learning_condition>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.condition_id).HasColumnType("bigint(20)");

            entity.Property(e => e.content_id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time).HasColumnType("datetime");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.dif).HasColumnType("varchar(5)");

            entity.Property(e => e.update_time).HasColumnType("datetime");
        });
        modelBuilder.Entity<t_task_bus_score>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag).HasColumnType("tinyint(3)");

            entity.Property(e => e.department).HasColumnType("varchar(50)");

            entity.Property(e => e.end_time).HasColumnType("datetime");

            entity.Property(e => e.result).HasColumnType("tinyint(3)");

            entity.Property(e => e.start_time).HasColumnType("datetime");

            entity.Property(e => e.status).HasColumnType("tinyint(3)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime(3)")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'");

            entity.Property(e => e.t_modify)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.task_bus_id).HasColumnType("bigint(20)");

            entity.Property(e => e.user_id).HasColumnType("bigint(20)");

            entity.Property(e => e.user_name).HasColumnType("varchar(50)");
        });
        modelBuilder.Entity<t_task_bus>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.airplane_type_key).HasColumnType("varchar(20)");

            entity.Property(e => e.airplane_type_value).HasColumnType("varchar(50)");

            entity.Property(e => e.class_hour).HasColumnType("int(11)");

            entity.Property(e => e.creator_id).HasColumnType("bigint(20)");

            entity.Property(e => e.creator_name).HasColumnType("varchar(255)");

            entity.Property(e => e.delete_flag).HasColumnType("tinyint(3)");

            entity.Property(e => e.level_key).HasColumnType("varchar(20)");

            entity.Property(e => e.level_value).HasColumnType("varchar(50)");

            entity.Property(e => e.original_id).HasColumnType("bigint(20)");

            entity.Property(e => e.plan_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime(3)")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime(3)")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.tag_display).HasColumnType("varchar(255)");

            entity.Property(e => e.task_desc).HasColumnType("text");

            entity.Property(e => e.task_name).HasColumnType("varchar(200)");

            entity.Property(e => e.task_type_key).HasColumnType("varchar(20)");

            entity.Property(e => e.task_type_value).HasColumnType("varchar(50)");

            entity.Property(e => e.type_level_key).HasColumnType("varchar(20)");

            entity.Property(e => e.type_level_value).HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<t_subject_bus>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.classify_key).HasColumnType("varchar(20)");

            entity.Property(e => e.classify_value).HasColumnType("varchar(50)");

            entity.Property(e => e.creator_id).HasColumnType("bigint(20)");

            entity.Property(e => e.creator_name).HasColumnType("varchar(50)");

            entity.Property(e => e.delete_flag).HasColumnType("tinyint(3)");

            entity.Property(e => e.description).HasColumnType("text");

            entity.Property(e => e.expect_result).HasColumnType("text");

            entity.Property(e => e.name).HasColumnType("varchar(200)");

            entity.Property(e => e.number).HasColumnType("varchar(100)");

            entity.Property(e => e.original_id).HasColumnType("bigint(20)");

            entity.Property(e => e.plane_type_key).HasColumnType("varchar(20)");

            entity.Property(e => e.plane_type_value).HasColumnType("varchar(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime(3)")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime(3)")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.tag_display).HasColumnType("varchar(200)");

            entity.Property(e => e.task_bus_id).HasColumnType("bigint(20)");

            entity.Property(e => e.version).HasColumnType("int(11)");
        });

        modelBuilder.Entity<t_plancourse_node_statistic>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.finish_count).HasColumnType("int(11)");

            entity.Property(e => e.finish_progress).HasColumnType("decimal(5,2)");

            entity.Property(e => e.node_extension).HasColumnType("varchar(5)");

            entity.Property(e => e.node_id).HasColumnType("bigint(20)");

            entity.Property(e => e.node_name).HasColumnType("varchar(100)");

            entity.Property(e => e.node_type).HasColumnType("varchar(20)");

            entity.Property(e => e.parent_id).HasColumnType("bigint(20)");

            entity.Property(e => e.plan_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_course>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_confidential).HasColumnType("varchar(5)");

            entity.Property(e => e.course_count)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.course_desc).HasColumnType("longtext");

            entity.Property(e => e.course_name).HasColumnType("varchar(50)");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.learning_time)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.src_id).HasColumnType("bigint(20)");

            entity.Property(e => e.thumbnail_path).HasColumnType("varchar(200)");

            entity.Property(e => e.update_by).HasColumnType("bigint(20)");

            entity.Property(e => e.update_time)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.user_name).HasColumnType("varchar(50)");

            entity.Property(e => e.user_number).HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<t_course_know_tag>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_id).HasColumnType("bigint(20)");

            entity.Property(e => e.tag_id).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<t_course_node_learning_log>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.attempt_time).HasColumnType("int(11)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.status_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create).HasColumnType("datetime")
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'"); ;

            entity.Property(e => e.t_modified).HasColumnType("datetime")
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate(); ;
        });

        modelBuilder.Entity<t_course_node_learning_status>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.attempt_number).HasColumnType("int(11)");
            entity.Property(e => e.node_id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_struct_id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time).HasColumnType("datetime")
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'"); ;

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.resource_count)
            .HasColumnType("int(11)")
            .HasDefaultValueSql("'0'");

            entity.Property(e => e.learning_page_number)
             .HasColumnType("int(11)")
             .HasDefaultValueSql("'0'");

            entity.Property(e => e.last_learning_time).HasColumnType("datetime");

            entity.Property(e => e.learning_time).HasColumnType("int(11)");
            entity.Property(e => e.sum_learning_time).HasColumnType("int(11)");

            entity.Property(e => e.node_status).HasColumnType("varchar(10)");

            entity.Property(e => e.resource_extension).HasColumnType("varchar(20)");

            entity.Property(e => e.record_id).HasColumnType("bigint(20)");

            entity.Property(e => e.plan_id).HasColumnType("bigint(20)");

            entity.Property(e => e.node_type).HasColumnType("varchar(20)");

            entity.Property(e => e.user_number).HasColumnType("varchar(50)");

            entity.Property(e => e.node_name).HasColumnType("varchar(200)");

            entity.Property(e => e.course_id).HasColumnType("bigint(20)");

            entity.Property(e => e.update_time).HasColumnType("datetime")
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_course_resource>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

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

            entity.Property(e => e.thumbnail_path).HasColumnType("varchar(200)");

            entity.Property(e => e.update_by).HasColumnType("bigint(20)");

            entity.Property(e => e.update_time)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });
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
        modelBuilder.Entity<t_course_struct>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_node_desc).HasColumnType("longtext");

            entity.Property(e => e.course_node_name).HasColumnType("varchar(100)");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_name).HasColumnType("varchar(20)");

            entity.Property(e => e.create_time)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.node_sort).HasColumnType("int(11)");

            entity.Property(e => e.node_type).HasColumnType("varchar(20)");

            entity.Property(e => e.parent_id).HasColumnType("bigint(20)");

            entity.Property(e => e.resource_count).HasColumnType("int(11)");

            entity.Property(e => e.struct_id).HasColumnType("bigint(20)");

            entity.Property(e => e.update_by).HasColumnType("bigint(20)");

            entity.Property(e => e.update_name).HasColumnType("varchar(20)");

            entity.Property(e => e.update_time)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_examination_manage>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.exam_div).HasColumnType("varchar(5)");

            entity.Property(e => e.exam_duration).HasColumnType("int(11)");

            entity.Property(e => e.exam_name).HasColumnType("varchar(200)");

            entity.Property(e => e.src_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_knowledge_tag>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            // entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            //  entity.Property(e => e.parent_id).HasColumnType("bigint(20)");

            entity.Property(e => e.src_id).HasColumnType("bigint(20)");

            entity.Property(e => e.tag).HasColumnType("varchar(50)");

            // entity.Property(e => e.tag_desc).HasColumnType("longtext");

            //entity.Property(e => e.tag_sort).HasColumnType("int(11)");

            // entity.Property(e => e.update_by).HasColumnType("bigint(20)");

            entity.Property(e => e.update_time)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_learning_record>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.content_id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.learning_progress).HasColumnType("varchar(20)");

            entity.Property(e => e.learning_sum_time)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.learning_time).HasColumnType("datetime");

            entity.Property(e => e.update_time)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.user_number).HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<t_plan_course_task_exam_ref>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.avg_learningtime)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.content_id).HasColumnType("bigint(20)");

            entity.Property(e => e.content_sort).HasColumnType("int(11)");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time).HasColumnType("datetime")
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'"); ;

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.dif).HasColumnType("varchar(5)");

            entity.Property(e => e.finish_rate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.plan_id).HasColumnType("bigint(20)");

            entity.Property(e => e.teacher_name).HasColumnType("varchar(50)");

            entity.Property(e => e.teacher_num).HasColumnType("varchar(50)");

            entity.Property(e => e.update_time).HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate(); ;
        });

        modelBuilder.Entity<t_program_course_ref>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.courseid).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.programid).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create).HasColumnType("datetime");

            entity.Property(e => e.t_modified).HasColumnType("datetime");
        });

        modelBuilder.Entity<t_program_subject_ref>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.programid).HasColumnType("bigint(20)");

            entity.Property(e => e.subjectid).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create).HasColumnType("datetime");

            entity.Property(e => e.t_modified).HasColumnType("datetime");
        });

        modelBuilder.Entity<t_resource_know_tag>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.resource_id).HasColumnType("bigint(20)");

            entity.Property(e => e.tag_id).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<t_struct_resource>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_resouce_id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_struct_id).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<t_subject_know_tag>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.subject_id).HasColumnType("bigint(20)");

            entity.Property(e => e.tag_id).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<t_train_subject>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_name).HasColumnType("varchar(50)");

            entity.Property(e => e.create_time).HasColumnType("datetime");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.expect_result).HasColumnType("longtext");

            entity.Property(e => e.plane_type).HasColumnType("varchar(5)");

            entity.Property(e => e.plane_type_key).HasColumnType("varchar(5)");

            entity.Property(e => e.train_desc).HasColumnType("longtext");

            entity.Property(e => e.train_kind).HasColumnType("varchar(20)");

            entity.Property(e => e.train_name).HasColumnType("varchar(200)");

            entity.Property(e => e.train_number).HasColumnType("varchar(100)");

            entity.Property(e => e.update_by).HasColumnType("bigint(20)");

            entity.Property(e => e.update_time).HasColumnType("datetime");
        });

        modelBuilder.Entity<t_training_plan>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_flag)
                .HasColumnType("tinyint(3)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.program_id)
                .HasColumnType("bigint(20)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_number).HasColumnType("varchar(50)");

            entity.Property(e => e.create_name).HasColumnType("varchar(50)");

            entity.Property(e => e.create_time).HasColumnType("datetime");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.end_time).HasColumnType("datetime");

            entity.Property(e => e.exam_flag)
                .HasColumnType("tinyint(3)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.finish_rate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.plan_desc).HasColumnType("longtext");

            entity.Property(e => e.plan_name).HasColumnType("varchar(300)");

            entity.Property(e => e.plan_status).HasColumnType("varchar(5)");

            entity.Property(e => e.publish_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.quit_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.start_time).HasColumnType("datetime");

            entity.Property(e => e.stu_count)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.task_flag)
                .HasColumnType("tinyint(3)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.update_time).HasColumnType("datetime");
        });

        modelBuilder.Entity<t_training_program>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.src_id).HasColumnType("bigint(20)");

            entity.Property(e => e.content_question).HasColumnType("longtext");

            entity.Property(e => e.content_visible_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.course_visible_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.create_time).HasColumnType("datetime");

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

            entity.Property(e => e.update_time).HasColumnType("datetime");

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
        });

        modelBuilder.Entity<t_trainingplan_stu>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.actual_duration).HasColumnType("varchar(20)");

            entity.Property(e => e.airplane).HasColumnType("varchar(20)");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time).HasColumnType("datetime");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.department).HasColumnType("varchar(20)");

            entity.Property(e => e.education).HasColumnType("varchar(20)");

            entity.Property(e => e.fly_status).HasColumnType("varchar(20)");

            entity.Property(e => e.photo_path).HasColumnType("varchar(250)");

            entity.Property(e => e.skill_level).HasColumnType("varchar(20)");

            entity.Property(e => e.trainingplan_id).HasColumnType("bigint(20)");

            entity.Property(e => e.user_number).HasColumnType("varchar(20)");

            entity.Property(e => e.update_time).HasColumnType("datetime");

            entity.Property(e => e.user_id).HasColumnType("bigint(20)");

            entity.Property(e => e.user_name).HasColumnType("varchar(20)");
        });

        modelBuilder.Entity<t_trainingplan_stustatistic>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_comrate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.exam_comrate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.task_comrate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.trainingplan_id).HasColumnType("bigint(20)");

            entity.Property(e => e.user_id).HasColumnType("bigint(20)");

            entity.Property(e => e.user_number)
                .IsRequired()
                .HasColumnType("varchar(50)");
        });
    }
}

