﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeopleManager.Core.Entity;

namespace PeopleManager.Infrastructure.Database.EntityConfigurations
{
    public class CertificateInfoConfiguration:IEntityTypeConfiguration<CertificateInfoEntity>
    {
        public void Configure(EntityTypeBuilder<CertificateInfoEntity> builder)
        {
            #region ::::: 公共字段 :::::
            builder.HasKey(x => x.Id);

            builder.Property(e => e.DeleteFlag).HasColumnType("tinyint(2)").HasDefaultValueSql("'0'");

            #endregion

            builder.HasOne(x => x.PersonInfo).WithMany(x => x.CertificateInfos).HasForeignKey(x => x.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
