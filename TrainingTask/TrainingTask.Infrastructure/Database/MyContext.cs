using Microsoft.EntityFrameworkCore;
using TrainingTask.Core.Entity;
using TrainingTask.Infrastructure.Database.EntityConfigurations;

namespace TrainingTask.Infrastructure.Database
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
        }

        /// <summary>
        /// 训练任务表
        /// </summary>
        public DbSet<TaskEntity> TaskInfos { get; set; }

        /// <summary>
        /// 训练科目表
        /// </summary>
        public DbSet<SubjectEntity> RelationInfos { get; set; }

        /// <summary>
        /// 任务科目关联表
        /// </summary>
        public DbSet<TaskSubjectRefEntity> TaskSubjectRefInfos { get; set; }

        /// <summary>
        /// 知识点表
        /// </summary>
        public DbSet<TagEntity> TagInfos { get; set; }

        /// <summary>
        /// 科目知识点关联表
        /// </summary>
        public DbSet<SubjectTagRefEntity> SubjectTagRefInfos { get; set; }

        /// <summary>
        /// 任务成绩表
        /// </summary>
        public DbSet<TaskScoreEntity> TaskScoreInfos { get; set; }

        /// <summary>
        /// 科目成绩表
        /// </summary>
        public DbSet<SubjectScoreEntity> SubjectScoreInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TaskConfiguration());
            modelBuilder.ApplyConfiguration(new SubjectConfiguration());
            modelBuilder.ApplyConfiguration(new TaskSubjectRefConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new SubjectTagRefConfiguration());
            modelBuilder.ApplyConfiguration(new TaskScoreConfiguration());
            modelBuilder.ApplyConfiguration(new SubjectScoreConfiguration());
        }
    }
}
