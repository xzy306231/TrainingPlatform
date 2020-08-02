using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public partial class pf_exam_paper_questionsContext : DbContext
{
    public pf_exam_paper_questionsContext()
    {
    }

    public pf_exam_paper_questionsContext(DbContextOptions<pf_exam_paper_questionsContext> options)
        : base(options)
    {
      
    }


    public virtual DbSet<t_config_paper_complexity> t_config_paper_complexity { get; set; }
    public virtual DbSet<t_file_path> t_file_path { get; set; }
    public virtual DbSet<t_knowledge_tag> t_knowledge_tag { get; set; }
    public virtual DbSet<t_question_basket> t_question_basket { get; set; }
    public virtual DbSet<t_question_knowledge_ref> t_question_knowledge_ref { get; set; }
    public virtual DbSet<t_question_option> t_question_option { get; set; }
    public virtual DbSet<t_question_option_bus> t_question_option_bus { get; set; }
    public virtual DbSet<t_question_statistic> t_question_statistic { get; set; }
    public virtual DbSet<t_question_tag_bus_ref> t_question_tag_bus_ref { get; set; }
    public virtual DbSet<t_questions> t_questions { get; set; }
    public virtual DbSet<t_questions_bus> t_questions_bus { get; set; }
    public virtual DbSet<t_tag_bus> t_tag_bus { get; set; }
    public virtual DbSet<t_test_papers> t_test_papers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //Scaffold-DbContext "server=192.168.1.143;userid=root;pwd=root;port=3306;database=pf_exam_paper_questions;sslmode=none;" Pomelo.EntityFrameworkCore.MySql -OutputDir Models -UseDatabaseNames -Force
            var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(this.GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
            var config = builder.Build();
            string strConn = config.GetConnectionString("conn");
            optionsBuilder.UseMySql(strConn);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<t_config_paper_complexity>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.difficulty).HasColumnType("decimal(5,2)");

            entity.Property(e => e.easy).HasColumnType("decimal(5,2)");

            entity.Property(e => e.general).HasColumnType("decimal(5,2)");

            entity.Property(e => e.paper_complexity).HasColumnType("varchar(5)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
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

        modelBuilder.Entity<t_question_basket>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.complexity).HasColumnType("varchar(10)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.question_id).HasColumnType("bigint(20)");

            entity.Property(e => e.question_score).HasColumnType("decimal(5, 2)")
            .HasDefaultValueSql("'0'");

            entity.Property(e => e.question_type_sort).HasColumnType("int(11)");

            entity.Property(e => e.question_sort).HasColumnType("int(11)");

            entity.Property(e => e.question_type).HasColumnType("varchar(10)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.test_paper_id).HasColumnType("bigint(20)");

            entity.Property(e => e.user_number).HasColumnType("varchar(50)");
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

            entity.Property(e => e.create_number).HasColumnType("varchar(50)");

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

        modelBuilder.Entity<t_question_option_bus>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_number).HasColumnType("varchar(50)");

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

        modelBuilder.Entity<t_question_statistic>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.questionid).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.use_count)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<t_question_tag_bus_ref>(entity =>
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

        modelBuilder.Entity<t_questions>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.answer_analyze).HasColumnType("longtext");

            entity.Property(e => e.approval_date)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.approval_remarks).HasColumnType("longtext");

            entity.Property(e => e.approval_status).HasColumnType("varchar(5)");

            entity.Property(e => e.approval_user_name).HasColumnType("varchar(50)");

            entity.Property(e => e.approval_user_number).HasColumnType("varchar(50)");

            entity.Property(e => e.complexity).HasColumnType("varchar(10)");

            entity.Property(e => e.create_name).HasColumnType("varchar(50)");

            entity.Property(e => e.create_number).HasColumnType("varchar(50)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.publish_flag)
                .HasColumnType("varchar(5)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.use_count)
               .HasColumnType("int(11)")
               .HasDefaultValueSql("'0'");

            entity.Property(e => e.question_answer).HasColumnType("longtext");

            entity.Property(e => e.question_confidential).HasColumnType("varchar(10)");

            entity.Property(e => e.question_desc).HasColumnType("longtext");

            entity.Property(e => e.question_title).HasColumnType("longtext");

            entity.Property(e => e.question_type).HasColumnType("varchar(10)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<t_questions_bus>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.answer_analyze).HasColumnType("longtext");

            entity.Property(e => e.complexity).HasColumnType("varchar(10)");

            entity.Property(e => e.create_name).HasColumnType("varchar(50)");

            entity.Property(e => e.create_number).HasColumnType("varchar(50)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.question_answer).HasColumnType("longtext");

            entity.Property(e => e.question_confidential).HasColumnType("varchar(10)");

            entity.Property(e => e.question_desc).HasColumnType("longtext");

            entity.Property(e => e.question_score).HasColumnType("decimal(5,2)");

            entity.Property(e => e.question_type_sort).HasColumnType("int(11)");

            entity.Property(e => e.question_sort).HasColumnType("int(11)");

            entity.Property(e => e.question_title).HasColumnType("longtext");

            entity.Property(e => e.question_type).HasColumnType("varchar(10)");

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

        modelBuilder.Entity<t_tag_bus>(entity =>
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

        modelBuilder.Entity<t_test_papers>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.approval_date).HasColumnType("datetime");

            entity.Property(e => e.approval_remarks).HasColumnType("longtext");

            entity.Property(e => e.approval_status).HasColumnType("varchar(5)");

            entity.Property(e => e.approval_user_name).HasColumnType("varchar(50)");

            entity.Property(e => e.approval_user_number).HasColumnType("varchar(50)");

            entity.Property(e => e.complexity).HasColumnType("varchar(5)");

            entity.Property(e => e.create_name).HasColumnType("varchar(50)");

            entity.Property(e => e.create_number).HasColumnType("varchar(50)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.exam_score).HasColumnType("decimal(5,2)");

            entity.Property(e => e.paper_confidential).HasColumnType("varchar(5)");

            entity.Property(e => e.paper_desc).HasColumnType("longtext");

            entity.Property(e => e.paper_title).HasColumnType("varchar(200)");

            entity.Property(e => e.question_count).HasColumnType("int(11)");

            entity.Property(e => e.share_flag)
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
    }
}

