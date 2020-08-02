using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;


public partial class pf_datastatisticContext : DbContext
{
    public pf_datastatisticContext()
    {
    }

    public pf_datastatisticContext(DbContextOptions<pf_datastatisticContext> options)
        : base(options)
    {
    }
    //Scaffold-DbContext "server=192.168.1.143;userid=root;pwd=root;port=3306;database=pf_datastatistic;sslmode=none;" Pomelo.EntityFrameworkCore.MySql -OutputDir Models -UseDatabaseNames -Force
    public static string ConnectionString => GetConnectionString();

    private static string GetConnectionString()
    {
        var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(new pf_datastatisticContext().GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
        var config = builder.Build();
        return config.GetConnectionString("conn");
    }
    public virtual DbSet<t_end_comment> t_end_comment { get; set; }
    public virtual DbSet<t_index_check_status> t_index_check_status { get; set; }
    public virtual DbSet<t_index_setting> t_index_setting { get; set; }
    public virtual DbSet<t_statistic_exam_correct_rank> t_statistic_exam_correct_rank { get; set; }
    public virtual DbSet<t_statistic_exam_knowledge> t_statistic_exam_knowledge { get; set; }
    public virtual DbSet<t_statistic_exam_pass_rank> t_statistic_exam_pass_rank { get; set; }
    public virtual DbSet<t_statistic_learningtime_rank> t_statistic_learningtime_rank { get; set; }
    public virtual DbSet<t_statistic_studata> t_statistic_studata { get; set; }
    public virtual DbSet<t_statistic_task_knowledge> t_statistic_task_knowledge { get; set; }
    public virtual DbSet<t_statistic_taskpass_rank> t_statistic_taskpass_rank { get; set; }
    public virtual DbSet<t_statistic_tdata> t_statistic_tdata { get; set; }
    public virtual DbSet<t_statistic_theory_knowledge> t_statistic_theory_knowledge { get; set; }
    public virtual DbSet<t_statistic_trainingtime> t_statistic_trainingtime { get; set; }

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
        modelBuilder.Entity<t_end_comment>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.end_comment).HasColumnType("longtext");

            entity.Property(e => e.suggestion).HasColumnType("longtext");
            entity.Property(e => e.div).HasColumnType("varchar(5)");
            entity.Property(e => e.end_level).HasColumnType("varchar(5)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_index_check_status>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.check_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'1'");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.plan_id).HasColumnType("bigint(20)");


            entity.Property(e => e.index_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_index_setting>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.index_dif).HasColumnType("varchar(5)");

            entity.Property(e => e.index_kind).HasColumnType("varchar(100)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_statistic_exam_knowledge>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

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

            entity.Property(e => e.tag_name).HasColumnType("varchar(50)");

            entity.Property(e => e.tag_rate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");
        });

        modelBuilder.Entity<t_statistic_exam_pass_rank>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.department).HasColumnType("varchar(50)");

            entity.Property(e => e.pass_rate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.user_name).HasColumnType("varchar(50)");

            entity.Property(e => e.user_number).HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<t_statistic_exam_correct_rank>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.correct_rate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.department).HasColumnType("varchar(50)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.user_name).HasColumnType("varchar(50)");

            entity.Property(e => e.user_number).HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<t_statistic_learningtime_rank>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.department).HasColumnType("varchar(45)");

            entity.Property(e => e.learning_time).HasColumnType("int(11)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.user_name).HasColumnType("varchar(50)");

            entity.Property(e => e.user_number).HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<t_statistic_studata>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.complete_cnum)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.complete_tasknum)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.complete_tnum)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.course_num)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.dept_name).HasColumnType("varchar(45)");

            entity.Property(e => e.learning_time)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.stu_number).HasColumnType("varchar(45)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.task_examrate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.task_num)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.th_examrate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.training_num)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.training_time)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");
        });

        modelBuilder.Entity<t_statistic_task_knowledge>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

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

            entity.Property(e => e.tag_name).HasColumnType("varchar(50)");

            entity.Property(e => e.tag_rate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");
        });

        modelBuilder.Entity<t_statistic_taskpass_rank>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.department).HasColumnType("varchar(50)");

            entity.Property(e => e.pass_rate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.user_name).HasColumnType("varchar(50)");

            entity.Property(e => e.user_number).HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<t_statistic_tdata>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.avg_learningtime)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.sum_learningtime)
                .HasColumnType("decimal(10,2)")
             .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.avg_tasktime)
                  .HasColumnType("decimal(10,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.sum_tasktime)
                 .HasColumnType("decimal(10,2)")
              .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.comtraining_pnum)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.comtraining_rate)
                  .HasColumnType("decimal(10,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.exam_passrate)
                 .HasColumnType("decimal(10,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.exam_rightrate)
                 .HasColumnType("decimal(10,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.exam_subrate)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.intraining_pnum)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.learning_comrate)
                 .HasColumnType("decimal(10,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.max_learningtime)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.min_learningtime)
               .HasColumnType("decimal(10,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.subject_comrate)
                  .HasColumnType("decimal(10,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.subject_passrate)
                 .HasColumnType("decimal(10,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.task_comrate)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.task_passrate)
                 .HasColumnType("decimal(10,2)")
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.training_pnum)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.training_tnum)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<t_statistic_theory_knowledge>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

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

            entity.Property(e => e.tag_name).HasColumnType("varchar(50)");

            entity.Property(e => e.tag_rate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValueSql("'0.00'");
        });

        modelBuilder.Entity<t_statistic_trainingtime>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

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

            entity.Property(e => e.t_month).HasColumnType("varchar(45)");

            entity.Property(e => e.t_year).HasColumnType("varchar(45)");

            entity.Property(e => e.training_num)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");
        });
    }
}

