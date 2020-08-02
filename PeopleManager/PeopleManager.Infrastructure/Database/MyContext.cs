using Microsoft.EntityFrameworkCore;
using PeopleManager.Core.Entity;
using PeopleManager.Infrastructure.Database.EntityConfigurations;

namespace PeopleManager.Infrastructure.Database
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
            Database.Migrate();
        }

        /// <summary>
        /// 个人信息
        /// </summary>
        public DbSet<PersonInfoEntity> PersonInfos { get; set; }

        /// <summary>
        /// 工作信息
        /// </summary>
        public DbSet<WorkInfoEntity> WorkInfos { get; set; }

        /// <summary>
        /// 执照信息
        /// </summary>
        public DbSet<CertificateInfoEntity> CertificateInfos { get; set; }

        /// <summary>
        /// 培训记录
        /// </summary>
        public DbSet<TrainingRecordEntity> TrainingRecords { get; set; }

        /// <summary>
        /// 证书信息
        /// </summary>
        public DbSet<LicenseInfoEntity> LicenseInfos { get; set; }

        /// <summary>
        /// 奖惩信息
        /// </summary>
        public DbSet<RewardsAndPunishmentEntity> RewardsAndPunishments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PersonInfoConfiguration());
            modelBuilder.ApplyConfiguration(new WorkInfoConfiguration());
            modelBuilder.ApplyConfiguration(new CertificateInfoConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingRecordConfiguration());
            modelBuilder.ApplyConfiguration(new LicenseInfoConfiguration());
            modelBuilder.ApplyConfiguration(new RewardsAndPunishmentConfiguration());
        }
    }
}
