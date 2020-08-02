using Courseware.Core.Entities;
using Courseware.Infrastructure.Database.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Courseware.Infrastructure.Database
{
    /// <summary>
    /// 
    /// </summary>
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        {
        }

        public DbSet<ResourceEntity> ResourceEntities { get; set; }

        public DbSet<KnowledgeTagEntity> TagEntities { get; set; }

        public DbSet<ResourceTagEntity> ResourceTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ResourceConfiguration());
            modelBuilder.ApplyConfiguration(new KnowledgeTagConfiguration());
            modelBuilder.ApplyConfiguration(new ResourceTagConfiguration());
        }
    }
}
