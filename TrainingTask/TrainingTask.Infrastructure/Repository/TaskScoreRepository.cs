using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApiUtil.Linq;
using Microsoft.EntityFrameworkCore;
using TrainingTask.Core.Entity;
using TrainingTask.Infrastructure.Database;

namespace TrainingTask.Infrastructure.Repository
{
    public class TaskScoreRepository : RepositoryBase<TaskScoreEntity>
    {
        public TaskScoreRepository(MyContext context) : base(context)
        {
        }

        public async Task<TaskScoreEntity> GetAsyncIncludeSubject(Expression<Func<TaskScoreEntity, bool>> predicate = null,
            bool isNoTracking = true)
        {
            var data = isNoTracking ? DbSet.Where(predicate).AsNoTracking() : DbSet.Where(predicate);
            return await data.Include(x => x.SubjectScores).FirstOrDefaultAsync();
        }

        public async Task<List<TaskScoreEntity>> GetListIncludeSubjectAsync(
            Expression<Func<TaskScoreEntity, bool>> predicate = null, string ordering = "", bool isNoTracking = true)
        {
            var data = isNoTracking ? DbSet.Where(predicate).AsNoTracking() : DbSet.Where(predicate);
            if (!string.IsNullOrEmpty(ordering))
            {
                data = data.OrderByBatch(ordering);
            }
            return await data.Include(x=>x.SubjectScores).ToListAsync();
        }
    }
}
