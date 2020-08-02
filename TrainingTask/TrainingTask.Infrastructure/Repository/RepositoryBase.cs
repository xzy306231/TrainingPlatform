using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApiUtil.Entities;
using ApiUtil.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TrainingTask.Core.Entity;
using TrainingTask.Infrastructure.Database;

namespace TrainingTask.Infrastructure.Repository
{
    public class RepositoryBase<T> where T : BaseEntity
    {
        protected readonly DbSet<T> DbSet;

        public MyContext DbContext { get; }

        public DatabaseFacade Database => DbContext.Database;

        public IQueryable<T> Entities => DbSet.AsQueryable().AsNoTracking();

        public RepositoryBase(MyContext context)
        {
            DbContext = context;
            DbSet = DbContext.Set<T>();
        }

        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }

        public bool Any(Expression<Func<T, bool>> whereLambda)
        {
            return DbSet.Where(whereLambda).Any();
        }

        public void Disposed()
        {
            throw new Exception("不允许在这里释放上下文，请在UnitOfWork中操作");
            //DbContext.Dispose();
        }

        #region 插入数据
        public bool Insert(T entity, bool isSaveChange = true)
        {
            DbSet.Add(entity);
            if (isSaveChange)
            {
                return SaveChanges() > 0;
            }
            return false;
        }

        public async Task<bool> InsertAsync(T entity, bool isSaveChange = true)
        {
            DbSet.Add(entity);
            if (isSaveChange)
            {
                return await SaveChangesAsync() > 0;
            }
            return false;
        }

        public bool Insert(List<T> entities, bool isSaveChange = true)
        {
            DbSet.AddRange(entities);
            if (isSaveChange)
            {
                return SaveChanges() > 0;
            }
            return false;
        }
        public async Task<bool> InsertAsync(List<T> entities, bool isSaveChange = true)
        {
            DbSet.AddRange(entities);
            if (isSaveChange)
            {
                return await SaveChangesAsync() > 0;
            }
            return false;
        }
        #endregion

        #region 删除

        public bool Delete(T entity, bool isSaveChange = true)
        {
            DbSet.Attach(entity);
            DbSet.Remove(entity);
            return isSaveChange && SaveChanges() > 0;
        }

        public bool Delete(List<T> entities, bool isSaveChange = true)
        {
            entities.ForEach(entity =>
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            });
            return isSaveChange && SaveChanges() > 0;
        }

        public virtual async Task<bool> DeleteAsync(T entity, bool isSaveChange = true)
        {

            DbSet.Attach(entity);
            DbSet.Remove(entity);
            return isSaveChange && await SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> DeleteAsync(List<T> entities, bool isSaveChange = true)
        {
            entities.ForEach(entity =>
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            });
            return isSaveChange && await SaveChangesAsync() > 0;
        }
        #endregion

        #region 更新数据

        public bool Update(T entity, bool isSaveChange = true, List<string> updatePropertyList = null, bool modified = true)
        {
            if (entity == null)
            {
                return false;
            }
            DbSet.Attach(entity);
            var entry = DbContext.Entry(entity);
            if (updatePropertyList == null)
            {
                entry.State = EntityState.Modified;//全字段更新
            }
            else
            {
                if (modified)
                {
                    updatePropertyList.ForEach(c => {
                        entry.Property(c).IsModified = true; //部分字段更新的写法
                    });
                }
                else
                {
                    entry.State = EntityState.Modified;//全字段更新
                    updatePropertyList.ForEach(c => {
                        entry.Property(c).IsModified = false; //部分字段不更新的写法,没测试可能有BUG
                    });
                }
            }
            if (isSaveChange)
            {
                return SaveChanges() > 0;
            }
            return false;
        }

        public bool Update(List<T> entities, bool isSaveChange = true)
        {
            if (entities == null || entities.Count == 0)
            {
                return false;
            }
            entities.ForEach(c => {
                Update(c, false);
            });
            if (isSaveChange)
            {
                return SaveChanges() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateAsync(T entity, bool isSaveChange = true, List<string> updatePropertyList = null, bool modified = true)
        {
            if (entity == null)
            {
                return false;
            }
            DbSet.Attach(entity);
            var entry = DbContext.Entry<T>(entity);
            if (updatePropertyList == null)
            {
                entry.State = EntityState.Modified;//全字段更新
            }
            else
            {
                if (modified)
                {
                    updatePropertyList.ForEach(c => {
                        entry.Property(c).IsModified = true; //部分字段更新的写法
                    });
                }
                else
                {
                    entry.State = EntityState.Modified;//全字段更新
                    updatePropertyList.ForEach(c => {
                        entry.Property(c).IsModified = false; //部分字段不更新的写法
                    });
                }
            }
            if (isSaveChange)
            {
                return await SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateAsync(List<T> entities, bool isSaveChange = true)
        {
            if (entities == null || entities.Count == 0)
            {
                return false;
            }
            entities.ForEach(c => {
                DbSet.Attach(c);
                DbContext.Entry<T>(c).State = EntityState.Modified;
            });
            if (isSaveChange)
            {
                return await SaveChangesAsync() > 0;
            }
            return false;
        }
        #endregion

        #region 查找

        public long Count(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                predicate = c => true;
            }
            return DbSet.LongCount(predicate);
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                predicate = c => true;
            }
            return await DbSet.LongCountAsync(predicate);
        }

        public T Get(object id)
        {
            if (id == null)
            {
                return default(T);
            }
            return DbSet.Find(id);
        }

        public T Get(Expression<Func<T, bool>> predicate = null, bool isNoTracking = true)
        {
            var data = isNoTracking ? DbSet.Where(predicate).AsNoTracking() : DbSet.Where(predicate);
            return data.FirstOrDefault();
        }

        public async Task<T> GetAsync(object id)
        {
            if (id == null)
            {
                return default(T);
            }
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate = null, bool isNoTracking = true)
        {
            var data = isNoTracking ? DbSet.Where(predicate).AsNoTracking() : DbSet.Where(predicate);
            return await data.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate = null, string ordering = "", bool isNoTracking = true)
        {
            var data = isNoTracking ? DbSet.Where(predicate).AsNoTracking() : DbSet.Where(predicate);
            if (!string.IsNullOrEmpty(ordering))
            {
                data = data.OrderByBatch(ordering);
            }
            return await data.ToListAsync();
        }

        public List<T> GetList(Expression<Func<T, bool>> predicate = null, string ordering = "", bool isNoTracking = true)
        {
            var data = isNoTracking ? DbSet.Where(predicate).AsNoTracking() : DbSet.Where(predicate);
            if (!string.IsNullOrEmpty(ordering))
            {
                data = data.OrderByBatch(ordering);
            }
            return data.ToList();
        }

        public async Task<IQueryable<T>> LoadAsync(Expression<Func<T, bool>> predicate = null, bool isNoTracking = true)
        {
            if (predicate == null)
            {
                predicate = c => true;
            }
            return await Task.Run(() => isNoTracking ? DbSet.Where(predicate).AsNoTracking() : DbSet.Where(predicate));
        }

        public IQueryable<T> Load(Expression<Func<T, bool>> predicate = null, bool isNoTracking = true)
        {
            if (predicate == null)
            {
                predicate = c => true;
            }
            return isNoTracking ? DbSet.Where(predicate).AsNoTracking() : DbSet.Where(predicate);
        }

        #region 分页查找

        /// <summary>
        /// 分页查询异步
        /// </summary>
        /// <param name="whereLambda">查询添加（可有，可无）</param>
        /// <param name="orderBy">排序条件（一定要有）</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="isOrder">排序正反</param>
        /// <param name="isNoTracking"></param>
        /// <returns></returns>
        public virtual async Task<PageData<T>> GetPageAsync<TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true)
        {
            IQueryable<T> data = isOrder ?
                DbSet.OrderBy(orderBy) :
                DbSet.OrderByDescending(orderBy);
            if (whereLambda != null)
            {
                //isNoTracking ? data.Where(whereLambda).AsNoTracking() : data.Where(whereLambda);
                data = DataOfPage(data, whereLambda, isNoTracking);
            }
            PageData<T> pageData = new PageData<T>
            {
                Totals = await data.CountAsync(),
                Rows = await data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
            };
            return pageData;
        }

        /// <summary>
        /// 分页查询异步
        /// </summary>
        /// <param name="whereLambda">查询添加（可有，可无）</param>
        /// <param name="ordering">排序条件（一定要有，多个用逗号隔开，倒序开头用-号）</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="isNoTracking"></param>
        /// <returns></returns>
        public virtual async Task<PageData<T>> GetPageAsync(Expression<Func<T, bool>> whereLambda, string ordering, int pageIndex, int pageSize, bool isNoTracking = true)
        {
            // 分页 一定注意： Skip 之前一定要 OrderBy
            if (string.IsNullOrEmpty(ordering))
            {
                ordering = nameof(T) + "Id";//默认以Id排序
            }
            var data = DbSet.OrderByBatch(ordering);
            if (whereLambda != null)
            {
                //isNoTracking ? data.Where(whereLambda).AsNoTracking() : data.Where(whereLambda);
                data = DataOfPage(data, whereLambda, isNoTracking);
            }
            //查看生成的sql，找到大数据下分页巨慢原因为order by 耗时
            //var sql = data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToSql();
            //File.WriteAllText(@"D:\sql.txt",sql);
            PageData<T> pageData = new PageData<T>
            {
                Totals = await data.CountAsync(),
                Rows = await data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
            };
            return pageData;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereLambda">查询添加（可有，可无）</param>
        /// <param name="ordering">排序条件（一定要有，多个用逗号隔开，倒序开头用-号）</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="isNoTracking"></param>
        /// <returns></returns>
        public virtual PageData<T> GetPage(Expression<Func<T, bool>> whereLambda, string ordering, int pageIndex, int pageSize, bool isNoTracking = true)
        {
            // 分页 一定注意： Skip 之前一定要 OrderBy
            if (string.IsNullOrEmpty(ordering))
            {
                ordering = nameof(T) + "Id";//默认以Id排序
            }
            var data = DbSet.OrderByBatch(ordering);
            if (whereLambda != null)
            {
                //isNoTracking ? data.Where(whereLambda).AsNoTracking() : data.Where(whereLambda);
                data = DataOfPage(data, whereLambda, isNoTracking);
            }
            PageData<T> pageData = new PageData<T>
            {
                Totals = data.Count(),
                Rows = data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };
            return pageData;
        }

        public virtual IQueryable<T> DataOfPage(IQueryable<T> data, Expression<Func<T, bool>> whereLambda,
            bool isNoTracking = true)
        {
            return isNoTracking ? data.Where(whereLambda).AsNoTracking() : data.Where(whereLambda);
        }
        #endregion
        #endregion
    }
}
