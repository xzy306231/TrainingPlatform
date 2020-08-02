using System;
using System.Linq;
using System.Threading.Tasks;
using Courseware.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Courseware.Infrastructure.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public class UnitOfWork : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public MyContext DbContext { get; set; }

        /// <summary>
        /// 资源仓储
        /// </summary>
        public ResourceRepository ResourceRepository { get; }

        /// <summary>
        /// 标签仓储
        /// </summary>
        public TagRepository TagRepository { get; }

        /// <summary>
        /// 中间表仓储
        /// </summary>
        public ResourceTagRepository ResourceTagRepository { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public UnitOfWork(MyContext dbContext)
        {
            DbContext = dbContext;
            ResourceRepository = new ResourceRepository(DbContext);
            TagRepository = new TagRepository(DbContext);
            ResourceTagRepository = new ResourceTagRepository(DbContext);
        }


        #region 仓储操作（提交事务保存SaveChanges(),回滚RollBackChanges(),释放资源Dispose()）
        /// <summary>
        /// 保存
        /// </summary>
        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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
