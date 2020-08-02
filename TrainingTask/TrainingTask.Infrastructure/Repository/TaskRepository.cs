using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrainingTask.Core.Entity;
using TrainingTask.Infrastructure.Database;

namespace TrainingTask.Infrastructure.Repository
{
    public class TaskRepository : RepositoryBase<TaskEntity>
    {
        public TaskRepository(MyContext context) : base(context)
        {

        }

        public async Task<TaskEntity> GetFullAsync(Expression<Func<TaskEntity, bool>> predicate = null, bool isNoTracking = true)
        {
            var data = isNoTracking ? DbSet.Where(predicate).AsNoTracking() : DbSet.Where(predicate);
            return await data.Include(x => x.SubjectRefEntities).ThenInclude(x => x.Subject)
                .ThenInclude(x => x.TagRefEntities).ThenInclude(x => x.Tag).FirstOrDefaultAsync();
        }
        public async Task<TaskEntity> GetIncludeSubjectsAsync(Expression<Func<TaskEntity, bool>> predicate = null, bool isNoTracking = true)
        {
            var data = isNoTracking ? DbSet.Where(predicate).AsNoTracking() : DbSet.Where(predicate);
            return await data.Include(x => x.SubjectRefEntities).ThenInclude(x=>x.Subject).FirstOrDefaultAsync();
        }

    }
}
