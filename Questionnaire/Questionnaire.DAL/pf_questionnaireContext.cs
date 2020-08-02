using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;


public partial class pf_questionnaireContext : DbContext
{
    public pf_questionnaireContext()
    {
    }

    public pf_questionnaireContext(DbContextOptions<pf_questionnaireContext> options)
        : base(options)
    {
    }
    public static string ConnectionString => GetConnectionString();

    private static string GetConnectionString()
    {
        var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(new pf_questionnaireContext().GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
        var config = builder.Build();
        return config.GetConnectionString("conn");
    }
    // public virtual DbSet<t_course> t_course { get; set; }
    public virtual DbSet<t_interaction_log> t_interaction_log { get; set; }
    public virtual DbSet<t_item_option> t_item_option { get; set; }
    public virtual DbSet<t_questionnaire> t_questionnaire { get; set; }
    public virtual DbSet<t_questionnaire_item> t_questionnaire_item { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location);
            var builder = new ConfigurationBuilder().SetBasePath(path).AddJsonFile("dalsettings.json");
            var config = builder.Build();
            string str = config.GetConnectionString("conn");
            optionsBuilder.UseMySql(str);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<t_course>(entity =>
        //{
        //    entity.Property(e => e.id).HasColumnType("bigint(20)");

        //    entity.Property(e => e.course_desc).HasColumnType("longtext");

        //    entity.Property(e => e.course_name).HasColumnType("varchar(50)");

        //    entity.Property(e => e.src_id).HasColumnType("bigint(20)");
        //});

        modelBuilder.Entity<t_interaction_log>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.interaction_result).HasColumnType("longtext");

            entity.Property(e => e.interactive_time).HasColumnType("datetime");

            entity.Property(e => e.participate_id).HasColumnType("bigint(20)");

            entity.Property(e => e.participate_name).HasColumnType("varchar(20)");

            entity.Property(e => e.questionnaire_id).HasColumnType("bigint(20)");

            entity.Property(e => e.questionnaire_item_id).HasColumnType("bigint(20)");

            entity.Property(e => e.item_option_id).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<t_item_option>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time).HasColumnType("datetime");

            entity.Property(e => e.createby).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.option_number).HasColumnType("varchar(5)");

            entity.Property(e => e.option_content).HasColumnType("longtext");

            entity.Property(e => e.questionnaire_item_id).HasColumnType("bigint(20)");

            entity.Property(e => e.update_by).HasColumnType("bigint(20)");

            entity.Property(e => e.update_time).HasColumnType("datetime");
        });

        modelBuilder.Entity<t_questionnaire>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.plan_id).HasColumnType("bigint(20)");

            entity.Property(e => e.course_id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time).HasColumnType("datetime");

            entity.Property(e => e.delete_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.expiry_time).HasColumnType("datetime").HasDefaultValueSql(null);

            entity.Property(e => e.start_time).HasColumnType("datetime").HasDefaultValueSql(null);

            entity.Property(e => e.theme).HasColumnType("varchar(500)");
            entity.Property(e => e.current_status).HasColumnType("varchar(5)");

            entity.Property(e => e.theme_desc).HasColumnType("longtext");

            entity.Property(e => e.update_by).HasColumnType("bigint(20)");

            entity.Property(e => e.update_time).HasColumnType("datetime");
        });

        modelBuilder.Entity<t_questionnaire_item>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time).HasColumnType("datetime");

            entity.Property(e => e.delete_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.item_sort).HasColumnType("int(11)");

            entity.Property(e => e.item_title).HasColumnType("varchar(200)");

            entity.Property(e => e.item_type).HasColumnType("varchar(5)");

            entity.Property(e => e.item_type_desc).HasColumnType("varchar(20)");

            entity.Property(e => e.max_answer_num).HasColumnType("int(11)");

            entity.Property(e => e.min_answer_num).HasColumnType("int(11)");

            entity.Property(e => e.must_answer_flag).HasColumnType("tinyint(2)");

            entity.Property(e => e.questionnnaire_id).HasColumnType("bigint(20)");

            entity.Property(e => e.update_by).HasColumnType("bigint(20)");

            entity.Property(e => e.update_time).HasColumnType("datetime");
        });
    }
}

