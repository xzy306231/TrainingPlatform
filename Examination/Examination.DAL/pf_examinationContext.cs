using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;


public partial class pf_examinationContext : DbContext
{
    public pf_examinationContext()
    {
    }

    public pf_examinationContext(DbContextOptions<pf_examinationContext> options) : base(options)
    {
    }
    public static string ConnectionString => GetConnectionString();

    private static string GetConnectionString()
    {
        var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(new pf_examinationContext().GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
        var config = builder.Build();
        return config.GetConnectionString("conn");
    }
    public virtual DbSet<t_answer_log> t_answer_log { get; set; }
    public virtual DbSet<t_correct_stuexam> t_correct_stuexam { get; set; }
    public virtual DbSet<t_examination_manage> t_examination_manage { get; set; }
    public virtual DbSet<t_examination_record> t_examination_record { get; set; }
    public virtual DbSet<t_examination_student> t_examination_student { get; set; }
    public virtual DbSet<t_file_path> t_file_path { get; set; }
    public virtual DbSet<t_grade_teacher> t_grade_teacher { get; set; }
    public virtual DbSet<t_knowledge_tag> t_knowledge_tag { get; set; }
    public virtual DbSet<t_paper_question_ref> t_paper_question_ref { get; set; }
    public virtual DbSet<t_question_knowledge_ref> t_question_knowledge_ref { get; set; }
    public virtual DbSet<t_question_option> t_question_option { get; set; }
    public virtual DbSet<t_questions> t_questions { get; set; }
    public virtual DbSet<t_statistic_exam_accrate> t_statistic_exam_accrate { get; set; }
    public virtual DbSet<t_statistic_exam_knowledge> t_statistic_exam_knowledge { get; set; }
    public virtual DbSet<t_statistic_option> t_statistic_option { get; set; }
    public virtual DbSet<t_statistic_question> t_statistic_question { get; set; }
    public virtual DbSet<t_statistic_subject> t_statistic_subject { get; set; }
    public virtual DbSet<t_statistic_subject_knowledge> t_statistic_subject_knowledge { get; set; }
    public virtual DbSet<t_statistic_texam> t_statistic_texam { get; set; }
    public virtual DbSet<t_subject_knowledge_ref> t_subject_knowledge_ref { get; set; }
    public virtual DbSet<t_task_log> t_task_log { get; set; }
    public virtual DbSet<t_test_papers> t_test_papers { get; set; }
    public virtual DbSet<t_training_subject> t_training_subject { get; set; }
    public virtual DbSet<t_training_task> t_training_task { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(this.GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
            var config = builder.Build();
            string strConn = config.GetConnectionString("conn1");
            optionsBuilder.UseMySql(strConn);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<t_answer_log>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.answer_result).HasColumnType("longtext");

            entity.Property(e => e.correct_flag).HasColumnType("varchar(5)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.item_id).HasColumnType("bigint(20)");

            entity.Property(e => e.option_id).HasColumnType("bigint(20)");

            entity.Property(e => e.record_id).HasColumnType("bigint(20)");

            entity.Property(e => e.score)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_statistic_exam_accrate>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.acc_index).HasColumnType("tinyint(2)");

            entity.Property(e => e.acc_name).HasColumnType("varchar(45)");

            entity.Property(e => e.acc_rate).HasColumnType("decimal(5,2)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.exam_id).HasColumnType("bigint(20)");

            entity.Property(e => e.exam_num).HasColumnType("varchar(45)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_statistic_subject_knowledge>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.exam_id).HasColumnType("bigint(20)");

            entity.Property(e => e.know_id).HasColumnType("bigint(20)");

            entity.Property(e => e.know_name).HasColumnType("varchar(50)");

            entity.Property(e => e.know_rate).HasColumnType("decimal(5,2)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_statistic_subject>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.exam_id).HasColumnType("bigint(20)");

            entity.Property(e => e.nopass_nums).HasColumnType("int(11)");

            entity.Property(e => e.pass_nums).HasColumnType("int(11)");

            entity.Property(e => e.right_rate).HasColumnType("decimal(5,2)");

            entity.Property(e => e.subject_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.task_id).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<t_subject_knowledge_ref>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.knowledge_tag_id).HasColumnType("bigint(20)");

            entity.Property(e => e.subject_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_correct_stuexam>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("int(11)");

            entity.Property(e => e.correct_date).HasColumnType("datetime");

            entity.Property(e => e.correct_statu)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'1'");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.examination_id).HasColumnType("int(11)");

            entity.Property(e => e.gradetea_id).HasColumnType("int(11)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.uesr_name).HasColumnType("varchar(45)");

            entity.Property(e => e.user_number).HasColumnType("varchar(45)");
        });

        modelBuilder.Entity<t_examination_manage>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_name).HasColumnType("varchar(50)");

            entity.Property(e => e.create_num).HasColumnType("varchar(50)");

            entity.Property(e => e.correct_status).HasColumnType("varchar(5)");

            entity.Property(e => e.approval_status).HasColumnType("varchar(5)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.used_flag)
           .HasColumnType("tinyint(2)")
           .HasDefaultValueSql("'0'");

            entity.Property(e => e.publish_flag)
            .HasColumnType("tinyint(2)")
            .HasDefaultValueSql("'0'");

            entity.Property(e => e.end_time).HasColumnType("datetime").HasDefaultValueSql(null);

            entity.Property(e => e.content_id).HasColumnType("bigint(20)");

            entity.Property(e => e.exam_div).HasColumnType("varchar(5)");

            entity.Property(e => e.exam_duration).HasColumnType("int(11)");

            entity.Property(e => e.exam_explain).HasColumnType("longtext");

            entity.Property(e => e.paper_confidential).HasColumnType("varchar(5)");

            entity.Property(e => e.exam_name).HasColumnType("varchar(200)");

            entity.Property(e => e.exam_status).HasColumnType("varchar(5)");

            entity.Property(e => e.pass_scores).HasColumnType("int(11)");

            entity.Property(e => e.start_time).HasColumnType("datetime").HasDefaultValueSql(null);

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_examination_record>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.pass_flag)
               .HasColumnType("tinyint(2)")
               .HasDefaultValueSql("'0'");

            entity.Property(e => e.pass_rate)
                  .HasColumnType("decimal(5,2)")
                  .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.end_time).HasColumnType("datetime").HasDefaultValueSql(null);

            entity.Property(e => e.examination_id).HasColumnType("bigint(20)");

            entity.Property(e => e.plan_id).HasColumnType("bigint(20)");

            entity.Property(e => e.record_status).HasColumnType("varchar(5)");

            entity.Property(e => e.department).HasColumnType("varchar(5)");

            entity.Property(e => e.task_comment).HasColumnType("longtext");

            entity.Property(e => e.start_time).HasColumnType("datetime").HasDefaultValueSql(null);

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.user_number).HasColumnType("varchar(50)");
            entity.Property(e => e.user_name).HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<t_examination_student>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.department).HasColumnType("varchar(50)");

            entity.Property(e => e.examination_id).HasColumnType("bigint(20)");

            entity.Property(e => e.plan_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.uesr_name).HasColumnType("varchar(50)");

            entity.Property(e => e.user_number).HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<t_file_path>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.dif).HasColumnType("varchar(5)");

            entity.Property(e => e.file_name).HasColumnType("varchar(100)");

            entity.Property(e => e.group_name).HasColumnType("varchar(25)");

            entity.Property(e => e.path).HasColumnType("varchar(200)");

            entity.Property(e => e.src_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_task_log>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.exam_result).HasColumnType("varchar(10)");

            entity.Property(e => e.do_flag).HasColumnType("varchar(5)");

            entity.Property(e => e.record_id).HasColumnType("bigint(20)");

            entity.Property(e => e.subject_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.task_id).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<t_grade_teacher>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.correct_num)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.examination_id).HasColumnType("bigint(20)");

            entity.Property(e => e.grade_teacher_name).HasColumnType("varchar(50)");

            entity.Property(e => e.grade_teacher_num).HasColumnType("varchar(50)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.total_num)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<t_knowledge_tag>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.src_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.tag).HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<t_paper_question_ref>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.question_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.test_paper_id).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<t_question_knowledge_ref>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.knowledge_tag_id).HasColumnType("bigint(20)");

            entity.Property(e => e.question_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_question_option>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.option_content).HasColumnType("longtext");

            entity.Property(e => e.option_number).HasColumnType("varchar(5)");

            entity.Property(e => e.question_id).HasColumnType("bigint(20)");

            entity.Property(e => e.right_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_questions>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.answer_analyze).HasColumnType("varchar(300)");

            entity.Property(e => e.complexity).HasColumnType("varchar(5)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.question_answer).HasColumnType("longtext");

            entity.Property(e => e.question_confidential).HasColumnType("varchar(5)");

            entity.Property(e => e.question_desc).HasColumnType("longtext");

            entity.Property(e => e.question_score).HasColumnType("int(11)");

            entity.Property(e => e.question_sort).HasColumnType("int(11)");

            entity.Property(e => e.question_title).HasColumnType("longtext");

            entity.Property(e => e.question_type).HasColumnType("varchar(5)");

            entity.Property(e => e.src_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.test_paper_id).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<t_statistic_exam_knowledge>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.exam_id).HasColumnType("bigint(20)");

            entity.Property(e => e.know_id).HasColumnType("bigint(20)");

            entity.Property(e => e.know_name).HasColumnType("varchar(50)");

            entity.Property(e => e.know_rate).HasColumnType("decimal(5,2)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_statistic_option>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.option_id).HasColumnType("bigint(20)");

            entity.Property(e => e.select_nums).HasColumnType("int(11)");

            entity.Property(e => e.select_rate).HasColumnType("decimal(5,2)");

            entity.Property(e => e.statistic_qid).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_statistic_question>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.exam_id).HasColumnType("bigint(20)");

            entity.Property(e => e.q_num).HasColumnType("int(11)");

            entity.Property(e => e.question_id).HasColumnType("bigint(20)");

            entity.Property(e => e.right_rate).HasColumnType("decimal(5,2)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_statistic_texam>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.avg_rightrate).HasColumnType("decimal(5,2)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.exam_id).HasColumnType("bigint(20)");

            entity.Property(e => e.exam_num).HasColumnType("int(11)");

            entity.Property(e => e.nopass_rate).HasColumnType("decimal(5,2)");

            entity.Property(e => e.pass_rate).HasColumnType("decimal(5,2)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.total_num).HasColumnType("int(11)");

            entity.Property(e => e.tscore).HasColumnType("decimal(5,2)");
        });

        modelBuilder.Entity<t_test_papers>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.exam_score).HasColumnType("int(11)");

            entity.Property(e => e.approval_remarks).HasColumnType("longtext");

            entity.Property(e => e.approval_date)
             .HasColumnType("datetime")
             .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
             .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.approval_status).HasColumnType("varchar(5)");

            entity.Property(e => e.approval_user_name).HasColumnType("varchar(50)");

            entity.Property(e => e.approval_user_number).HasColumnType("varchar(50)");

            entity.Property(e => e.question_count).HasColumnType("int(11)");

            entity.Property(e => e.examination_id).HasColumnType("bigint(20)");

            entity.Property(e => e.paper_confidential).HasColumnType("varchar(5)");

            entity.Property(e => e.paper_desc).HasColumnType("longtext");

            entity.Property(e => e.paper_title).HasColumnType("varchar(200)");

            entity.Property(e => e.src_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_training_subject>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.expect_result).HasColumnType("longtext");

            entity.Property(e => e.plane_type).HasColumnType("varchar(5)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.task_id).HasColumnType("bigint(20)");

            entity.Property(e => e.train_desc).HasColumnType("longtext");

            entity.Property(e => e.train_kind).HasColumnType("varchar(5)");

            entity.Property(e => e.train_name).HasColumnType("varchar(200)");

            entity.Property(e => e.train_number).HasColumnType("varchar(100)");
        });

        modelBuilder.Entity<t_training_task>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.examination_id).HasColumnType("bigint(20)");

            entity.Property(e => e.kind_level).HasColumnType("varchar(20)");

            entity.Property(e => e.rank_level).HasColumnType("varchar(20)");

            entity.Property(e => e.src_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.task_name).HasColumnType("varchar(200)");

            entity.Property(e => e.task_type).HasColumnType("varchar(20)");
            entity.Property(e => e.plane_type).HasColumnType("varchar(20)");
        });
    }
}

