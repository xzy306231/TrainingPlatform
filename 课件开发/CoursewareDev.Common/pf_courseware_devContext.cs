using Microsoft.EntityFrameworkCore;

public partial class pf_courseware_devContext : DbContext
{
    public pf_courseware_devContext()
    {
    }

    public pf_courseware_devContext(DbContextOptions<pf_courseware_devContext> options)
        : base(options)
    {
        
    }

    public virtual DbSet<__efmigrationshistory> __efmigrationshistory { get; set; }
    public virtual DbSet<t_cooperation_users> t_cooperation_users { get; set; }
    public virtual DbSet<t_course_resource> t_course_resource { get; set; }
    public virtual DbSet<t_courseware> t_courseware { get; set; }
    public virtual DbSet<t_courseware_page> t_courseware_page { get; set; }
    public virtual DbSet<t_courseware_page_bus> t_courseware_page_bus { get; set; }
    public virtual DbSet<t_knowledge_tag> t_knowledge_tag { get; set; }
    public virtual DbSet<t_resource_tag_ref> t_resource_tag_ref { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //Scaffold-DbContext "server=192.168.1.149;userid=root;pwd=root;port=3306;database=pf_courseware_dev;sslmode=none;" Pomelo.EntityFrameworkCore.MySql -OutputDir Models -UseDatabaseNames -Force
            optionsBuilder.UseMySql("server=192.168.1.149;userid=root;pwd=root;port=3306;database=pf_course_resource;sslmode=none;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<__efmigrationshistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId)
                .HasName("PRIMARY");

            entity.Property(e => e.MigrationId).HasColumnType("varchar(95)");

            entity.Property(e => e.ProductVersion)
                .IsRequired()
                .HasColumnType("varchar(32)");
        });

        modelBuilder.Entity<t_cooperation_users>(entity =>
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

            entity.Property(e => e.user_name).HasColumnType("varchar(50)");

            entity.Property(e => e.user_number).HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<t_course_resource>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.SCORM_version).HasColumnType("text");

            entity.Property(e => e.check_date).HasColumnType("datetime");

            entity.Property(e => e.check_remark).HasColumnType("text");

            entity.Property(e => e.check_status).HasColumnType("text");

            entity.Property(e => e.checker_id).HasColumnType("bigint(20)");

            entity.Property(e => e.checker_name).HasColumnType("text");

            entity.Property(e => e.creator_id).HasColumnType("bigint(20)");

            entity.Property(e => e.creator_name).HasColumnType("text");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.file_size).HasColumnType("bigint(20)");

            entity.Property(e => e.file_size_display).HasColumnType("text");

            entity.Property(e => e.file_suffix).HasColumnType("text");

            entity.Property(e => e.group_name).HasColumnType("text");

            entity.Property(e => e.md5_code).HasColumnType("text");

            entity.Property(e => e.original_url).HasColumnType("text");

            entity.Property(e => e.path_to_folder).HasColumnType("text");

            entity.Property(e => e.path_to_index).HasColumnType("text");

            entity.Property(e => e.resource_desc).HasColumnType("text");

            entity.Property(e => e.resource_duration).HasColumnType("int(11)");

            entity.Property(e => e.resource_level).HasColumnType("text");

            entity.Property(e => e.resource_name)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.resource_tags_display).HasColumnType("text");

            entity.Property(e => e.resource_type).HasColumnType("text");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.thumbnail_path).HasColumnType("text");

            entity.Property(e => e.title_from_manifest).HasColumnType("text");

            entity.Property(e => e.transf_type).HasColumnType("text");

            entity.Property(e => e.transform_url).HasColumnType("text");
        });

        modelBuilder.Entity<t_courseware>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.cooperation_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.courseware_desc).HasColumnType("longtext");

            entity.Property(e => e.courseware_title).HasColumnType("varchar(100)");

            entity.Property(e => e.create_id).HasColumnType("bigint(20)");

            entity.Property(e => e.create_name).HasColumnType("varchar(50)");

            entity.Property(e => e.create_number).HasColumnType("varchar(50)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.file_size)
               .HasColumnType("int(11)")
               .HasDefaultValueSql("'0'");

            entity.Property(e => e.publish_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.thumbnail_path).HasColumnType("varchar(200)");

            entity.Property(e => e.resource_confidential).HasColumnType("varchar(5)");

            entity.Property(e => e.update_number).HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<t_courseware_page>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.courseware_id).HasColumnType("bigint(20)");

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

        modelBuilder.Entity<t_knowledge_tag>(entity =>
        {
            entity.Property(e => e.id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.original_id).HasColumnType("bigint(20)");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.tag)
                .IsRequired()
                .HasColumnType("text");
        });

        modelBuilder.Entity<t_resource_tag_ref>(entity =>
        {
            entity.HasKey(e => new { e.resource_id, e.tag_id })
                .HasName("PRIMARY");

            entity.HasIndex(e => e.tag_id);

            entity.Property(e => e.resource_id).HasColumnType("bigint(20)");

            entity.Property(e => e.tag_id).HasColumnType("bigint(20)");

            entity.Property(e => e.delete_flag)
                .HasColumnType("tinyint(2)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.t_create)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.Property(e => e.t_modified)
                .HasColumnType("datetime")
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            entity.HasOne(d => d.resource_)
                .WithMany(p => p.t_resource_tag_ref)
                .HasForeignKey(d => d.resource_id);

            entity.HasOne(d => d.tag_)
                .WithMany(p => p.t_resource_tag_ref)
                .HasForeignKey(d => d.tag_id);
        });
    }
}

