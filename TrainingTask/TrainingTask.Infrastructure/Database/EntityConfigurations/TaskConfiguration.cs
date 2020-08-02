using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingTask.Core.Entity;

namespace TrainingTask.Infrastructure.Database.EntityConfigurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
    {
        public void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Copy).HasDefaultValue(0);
            builder.Property(e => e.FinishPercent).HasDefaultValue(0);
            builder.Property(e => e.PassPercent).HasDefaultValue(0);
            builder.Property(e => e.DurationAvg).HasDefaultValue(0);

            #region ::::: 公共字段 :::::

            builder.Property(e => e.DeleteFlag).HasColumnType("tinyint(2)").HasDefaultValueSql("'0'");

            builder.Property(e => e.CreateTime).ValueGeneratedOnAdd();

            builder.Property(e => e.ModifiedTime).ValueGeneratedOnUpdate();

            #endregion

        }
    }
}
