using Courseware.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courseware.Infrastructure.Database.EntityConfigurations
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourceTagConfiguration : IEntityTypeConfiguration<ResourceTagEntity>
    {
        public void Configure(EntityTypeBuilder<ResourceTagEntity> builder)
        {
            builder.HasKey(x => new{x.ResourceId,x.TagId});
            //builder.HasOne<ResourceEntity>(r=>r.Resource).WithMany(rt=>rt.ResourceTags).HasForeignKey(rt=>rt.ResourceId).

            #region ::::: 公共字段 :::::

            builder.Property(e => e.DeleteFlag).HasColumnType("tinyint(2)").HasDefaultValueSql("'0'");

            builder.Property(e => e.CreateTime).IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(e => e.ModifiedTime).IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            #endregion
        }
    }
}
