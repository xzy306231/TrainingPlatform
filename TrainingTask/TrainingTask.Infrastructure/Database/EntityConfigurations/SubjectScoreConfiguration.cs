using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingTask.Core.Entity;

namespace TrainingTask.Infrastructure.Database.EntityConfigurations
{
    public class SubjectScoreConfiguration : IEntityTypeConfiguration<SubjectScoreEntity>
    {
        public void Configure(EntityTypeBuilder<SubjectScoreEntity> builder)
        {
            builder.HasKey(e => e.Id);

            #region ::::: 公共字段 :::::

            builder.Property(e => e.DeleteFlag).HasColumnType("tinyint(2)").HasDefaultValueSql("'0'");

            builder.Property(e => e.CreateTime).ValueGeneratedOnAdd();

            builder.Property(e => e.ModifiedTime).ValueGeneratedOnUpdate();

            #endregion

            builder.HasOne(x => x.TaskScore).WithMany(x => x.SubjectScores).HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
