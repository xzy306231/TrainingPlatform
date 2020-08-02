using Courseware.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courseware.Infrastructure.Database.EntityConfigurations
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourceConfiguration :IEntityTypeConfiguration<ResourceEntity>
    {
        public void Configure(EntityTypeBuilder<ResourceEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(e => e.ResourceName).IsRequired();

            builder.Property(e => e.CheckerId).HasColumnType("bigint(20)");

            #region ::::: 公共字段 :::::

            builder.HasKey(e => e.Id);

            builder.Property(e => e.DeleteFlag).HasColumnType("tinyint(2)").HasDefaultValueSql("'0'");

            builder.Property(e => e.CreateTime).IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(e => e.ModifiedTime).IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            #endregion
        }
    }
}
