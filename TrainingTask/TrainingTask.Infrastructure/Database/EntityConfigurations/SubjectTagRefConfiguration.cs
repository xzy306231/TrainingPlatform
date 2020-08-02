using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingTask.Core.Entity;

namespace TrainingTask.Infrastructure.Database.EntityConfigurations
{
    public class SubjectTagRefConfiguration :IEntityTypeConfiguration<SubjectTagRefEntity>
    {
        public void Configure(EntityTypeBuilder<SubjectTagRefEntity> builder)
        {
            builder.HasKey(x => new {x.SubjectId, x.TagId});

            #region ::::: 公共字段 :::::

            builder.Property(e => e.DeleteFlag).HasColumnType("tinyint(2)").HasDefaultValueSql("'0'");

            builder.Property(e => e.CreateTime).ValueGeneratedOnAdd();

            builder.Property(e => e.ModifiedTime).ValueGeneratedOnUpdate();

            #endregion
        }
    }
}
