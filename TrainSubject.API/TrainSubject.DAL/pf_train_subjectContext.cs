using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

public partial class pf_train_subjectContext : DbContext
{
    public pf_train_subjectContext()
    {
    }

    public pf_train_subjectContext(DbContextOptions<pf_train_subjectContext> options)
        : base(options)
    {
    }
    public static string ConnectionString => GetConnectionString();

    private static string GetConnectionString()
    {
        var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(new pf_train_subjectContext().GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
        var config = builder.Build();
        return config.GetConnectionString("conn");
    }
    public virtual DbSet<t_knowledge_tag> t_knowledge_tag { get; set; }
    public virtual DbSet<t_train_subject> t_train_subject { get; set; }
    public virtual DbSet<t_subject_know_tag> t_subject_know_tag { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(this.GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
        var config = builder.Build();
        optionsBuilder.UseMySql(config.GetConnectionString("conn"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<t_subject_know_tag>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.subject_id).HasColumnType("bigint(20)");

            entity.Property(e => e.tag_id).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<t_knowledge_tag>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time).HasColumnType("datetime");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.parent_id).HasColumnType("bigint(20)");

            entity.Property(e => e.src_id).HasColumnType("bigint(20)");

            entity.Property(e => e.tag).HasColumnType("varchar(50)");

            entity.Property(e => e.tag_desc).HasColumnType("longtext");

            entity.Property(e => e.tag_sort).HasColumnType("int(11)");

            entity.Property(e => e.update_by).HasColumnType("bigint(20)");

            entity.Property(e => e.update_time).HasColumnType("datetime");
        });

        modelBuilder.Entity<t_train_subject>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time).HasColumnType("datetime");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.expect_result).HasColumnType("longtext");
            entity.Property(e => e.plane_type_key).HasColumnType("varchar(5)");

            entity.Property(e => e.plane_type).HasColumnType("varchar(5)");
            entity.Property(e => e.create_name).HasColumnType("varchar(50)");

            entity.Property(e => e.train_desc).HasColumnType("longtext");

            entity.Property(e => e.train_kind).HasColumnType("varchar(5)");

            entity.Property(e => e.train_name).HasColumnType("varchar(200)");

            entity.Property(e => e.train_number).HasColumnType("varchar(100)");

            entity.Property(e => e.update_by).HasColumnType("bigint(20)");

            entity.Property(e => e.update_time).HasColumnType("datetime");
        });

    }

}
