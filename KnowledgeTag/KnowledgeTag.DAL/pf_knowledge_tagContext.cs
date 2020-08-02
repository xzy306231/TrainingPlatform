using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;


public partial class pf_knowledge_tagContext : DbContext
{
    public pf_knowledge_tagContext()
    {
    }

    public pf_knowledge_tagContext(DbContextOptions<pf_knowledge_tagContext> options)
        : base(options)
    {
    }
    public static string ConnectionString => GetConnectionString();

    private static string GetConnectionString()
    {
        var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(new pf_knowledge_tagContext().GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
        var config = builder.Build();
        return config.GetConnectionString("conn");
    }
    public virtual DbSet<t_knowledge_tag> t_knowledge_tag { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(this.GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
            var config = builder.Build();
            string strCon = config.GetConnectionString("conn");
            //strCon = AESHelper.Decrypt(strCon);
            //IConfigurationSection strUserName = config.GetSection("UserName");
            optionsBuilder.UseMySql(strCon);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<t_knowledge_tag>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_by).HasColumnType("bigint(20)");

            entity.Property(e => e.create_time).HasColumnType("datetime");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.parent_id).HasColumnType("bigint(20)");

            entity.Property(e => e.tag).HasColumnType("varchar(50)");

            entity.Property(e => e.tag_desc).HasColumnType("longtext");

            entity.Property(e => e.tag_sort).HasColumnType("int(11)");

            entity.Property(e => e.update_by).HasColumnType("bigint(20)");

            entity.Property(e => e.update_time).HasColumnType("datetime");
        });
    }
}

