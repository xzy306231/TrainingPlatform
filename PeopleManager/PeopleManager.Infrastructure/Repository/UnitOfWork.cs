using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PeopleManager.Infrastructure.Database;

namespace PeopleManager.Infrastructure.Repository
{
    public class UnitOfWork : IDisposable
    {
        public MyContext DbContext { get; set; }

        /// <summary>
        /// 人员信息仓储
        /// </summary>
        public PersonInfoRepository PersonInfoRepository { get; }

        /// <summary>
        /// 工作信息仓储
        /// </summary>
        public WorkInfoRepository WorkInfoRepository { get; }

        /// <summary>
        /// 执照信息仓储
        /// </summary>
        public CertificateInfoRepository CertificateInfoRepository { get; }

        /// <summary>
        /// 培训记录仓储
        /// </summary>
        public TrainingRecordRepository TrainingRecordRepository { get; }

        /// <summary>
        /// 证照信息仓储
        /// </summary>
        public LicenseInfoRepository LicenseInfoRepository { get; set; }

        /// <summary>
        /// 奖惩记录仓储
        /// </summary>
        public RewardsAndPunishmentRepository RewardsAndPunishmentRepository { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public UnitOfWork(MyContext dbContext)
        {
            DbContext = dbContext;
            PersonInfoRepository = new PersonInfoRepository(DbContext);
            WorkInfoRepository = new WorkInfoRepository(DbContext);
            CertificateInfoRepository = new CertificateInfoRepository(DbContext);
            TrainingRecordRepository = new TrainingRecordRepository(DbContext);
            LicenseInfoRepository = new LicenseInfoRepository(DbContext);
            RewardsAndPunishmentRepository = new RewardsAndPunishmentRepository(DbContext);
        }


        #region 仓储操作（提交事务保存SaveChanges(),回滚RollBackChanges(),释放资源Dispose()）
        /// <summary>
        /// 保存
        /// </summary>
        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public void RollBackChanges()
        {
            var items = DbContext.ChangeTracker.Entries().ToList();
            items.ForEach(o => o.State = EntityState.Unchanged);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    DbContext.Dispose();//随着工作单元的销毁而销毁
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IDbContextTransaction BeginTransaction()
        {
            var scope = DbContext.Database.BeginTransaction();
            return scope;
        }
        #endregion
    }
}
