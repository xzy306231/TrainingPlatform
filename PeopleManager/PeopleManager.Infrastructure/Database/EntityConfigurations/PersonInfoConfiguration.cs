using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeopleManager.Core.Entity;

namespace PeopleManager.Infrastructure.Database.EntityConfigurations
{
    public class PersonInfoConfiguration : IEntityTypeConfiguration<PersonInfoEntity>
    {
        public void Configure(EntityTypeBuilder<PersonInfoEntity> builder)
        {
            #region ::::: 公共字段 :::::

            builder.HasKey(x => x.Id);

            builder.Property(e => e.DeleteFlag).HasColumnType("tinyint(2)").HasDefaultValueSql("'0'");
            //builder.Property(e=>e.CreateTime).HasComputedColumnSql()

            #endregion

            builder.Property(e => e.OriginalId).IsRequired().HasColumnType("bigint(20)");
            builder.Property(e => e.UserName).IsRequired();
            builder.Property(e => e.UserNumber).IsRequired();
            builder.Property(e => e.UserPhone).IsRequired();
            builder.Property(e => e.SecLevel).HasColumnType("tinyint(2)");
            builder.Property(e => e.StudentFlag).HasColumnType("tinyint(2)").HasDefaultValueSql("'1'");
            builder.Property(e => e.StudentFlag).HasColumnType("tinyint(2)").HasDefaultValueSql("'0'");
            builder.Property(e => e.TeacherFlag).HasColumnType("tinyint(2)").HasDefaultValueSql("'0'");
        }
    }
}
