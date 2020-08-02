using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeopleManager.Core.Entity;

namespace PeopleManager.Infrastructure.Database.EntityConfigurations
{
    public class RewardsAndPunishmentConfiguration :IEntityTypeConfiguration<RewardsAndPunishmentEntity>
    {
        public void Configure(EntityTypeBuilder<RewardsAndPunishmentEntity> builder)
        {
            #region ::::: 公共字段 :::::
            builder.HasKey(x => x.Id);

            builder.Property(e => e.DeleteFlag).HasColumnType("tinyint(2)").HasDefaultValueSql("'0'");

            #endregion

            builder.HasOne(x => x.PersonInfo).WithMany(x => x.RewardsAndPunishments).HasForeignKey(x => x.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
