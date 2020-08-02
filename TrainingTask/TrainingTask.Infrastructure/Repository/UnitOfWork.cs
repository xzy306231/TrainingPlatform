using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TrainingTask.Infrastructure.Database;

namespace TrainingTask.Infrastructure.Repository
{
    public class UnitOfWork : IDisposable
    {
        public MyContext DbContext { get; set; }

        /// <summary>
        /// 训练任务仓储
        /// </summary>
        public TaskRepository TaskRep { get; }

        /// <summary>
        /// 训练科目仓储
        /// </summary>
        public SubjectRepository SubjectRep { get; }

        /// <summary>
        /// 训练任务科目关联表仓储
        /// </summary>
        public TaskSubjectRefRepository TaskSubjectRefRep { get; set; }

        /// <summary>
        /// 知识点仓储
        /// </summary>
        public TagRepository TagRep { get; set; }

        /// <summary>
        /// 训练任务知识点关联表仓储
        /// </summary>
        public SubjectTagRefRepository SubjectTagRefRep { get; set; }

        /// <summary>
        /// 任务成绩仓储
        /// </summary>
        public TaskScoreRepository TaskScoreRep { get; }

        /// <summary>
        /// 科目成绩仓储
        /// </summary>
        public SubjectScoreRepository SubjectScoreRep { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public UnitOfWork(MyContext dbContext)
        {
            DbContext = dbContext;
            TaskRep = new TaskRepository(DbContext);
            SubjectRep = new SubjectRepository(DbContext);
            TaskSubjectRefRep = new TaskSubjectRefRepository(DbContext);
            TagRep = new TagRepository(DbContext);
            SubjectTagRefRep = new SubjectTagRefRepository(DbContext);
            TaskScoreRep = new TaskScoreRepository(DbContext);
            SubjectScoreRep = new SubjectScoreRepository(DbContext);
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
